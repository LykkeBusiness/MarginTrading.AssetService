// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Exceptions;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using TimeZoneConverter;

namespace MarginTrading.AssetService.Services
{
    public class MarketSettingsService : IMarketSettingsService
    {
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;
        private readonly CorrelationContextAccessor _correlationContextAccessor;

        public MarketSettingsService(
            IMarketSettingsRepository marketSettingsRepository,
            IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService,
            CorrelationContextAccessor correlationContextAccessor)
        {
            _marketSettingsRepository = marketSettingsRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public Task<MarketSettings> GetByIdAsync(string id)
            => _marketSettingsRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<MarketSettings>> GetAllMarketSettingsAsync()
            => _marketSettingsRepository.GetAllMarketSettingsAsync();

        public async Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettingsCreateOrUpdateDto model, string username)
        {
            var creationResult = CreateMarketSettingsWithDefaults(model);

            if (creationResult.IsFailed)
                return creationResult;

            var validationResult = ValidateSettings(creationResult.Value);

            if (validationResult.IsFailed)
                return validationResult;

            var addResult = await _marketSettingsRepository.AddAsync(creationResult.Value);

            if (addResult.IsFailed)
                return addResult;

            await _auditService.CreateAuditRecord(AuditEventType.Creation, username, creationResult.Value);

            await PublishMarketSettingsChangedEvent(null, creationResult.Value, username, ChangeType.Creation);

            return new Result<MarketSettingsErrorCodes>();
        }

        public async Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettingsCreateOrUpdateDto model, string username)
        {
            var creationResult = CreateMarketSettingsWithDefaults(model);
            
            if (creationResult.IsFailed)
                return creationResult;
            
            var currentSettings = await _marketSettingsRepository.GetByIdAsync(creationResult.Value.Id);

            if (currentSettings == null)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);

            var validationResult = ValidateSettings(creationResult.Value, currentSettings);

            if (validationResult.IsFailed)
                return validationResult;

            var updateResult = await _marketSettingsRepository.UpdateAsync(creationResult.Value);

            if (updateResult.IsFailed)
                return updateResult;

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, creationResult.Value, currentSettings);

            await PublishMarketSettingsChangedEvent(currentSettings, creationResult.Value, username, ChangeType.Edition);

            return new Result<MarketSettingsErrorCodes>();
        }

        public async Task<Result<MarketSettingsErrorCodes>> DeleteAsync(string id, string username)
        {
            var existing = await _marketSettingsRepository.GetByIdAsync(id);

            if (existing == null)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);
            
            if(await _marketSettingsRepository.MarketSettingsAssignedToAnyProductAsync(id))
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.CannotDeleteMarketSettingsAssignedToAnyProduct);

            var deleteResult = await _marketSettingsRepository.DeleteAsync(id);

            if (deleteResult.IsFailed)
                return deleteResult;

            await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, existing);
            
            await PublishMarketSettingsChangedEvent(existing, null, username, ChangeType.Deletion);

            return new Result<MarketSettingsErrorCodes>();
        }

        private async Task PublishMarketSettingsChangedEvent(MarketSettings oldSettings, MarketSettings newSettings, string username, ChangeType changeType)
        {
            await _cqrsMessageSender.SendEvent(new MarketSettingsChangedEvent
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = _correlationContextAccessor.GetOrGenerateCorrelationId(),
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldMarketSettings = _convertService.Convert<MarketSettings, MarketSettingsContract>(oldSettings),
                NewMarketSettings = _convertService.Convert<MarketSettings, MarketSettingsContract>(newSettings)
            });
        }
        
        private static Result<MarketSettingsErrorCodes> ValidateSettings(MarketSettings newSettings, MarketSettings existingSettings = null)
        {
            if (newSettings.DividendsLong < 0 || newSettings.DividendsLong > 200)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsLongValue);

            if (newSettings.DividendsShort < 0 || newSettings.DividendsShort > 200)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsShortValue);

            if (newSettings.Dividends871M < 0 || newSettings.Dividends871M > 100)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividends871MValue);

            if (existingSettings == null) 
                return new Result<MarketSettingsErrorCodes>();

            // This is the current day taking into account the timezone
            var currentDay = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TZConvert.GetTimeZoneInfo(existingSettings.MarketSchedule.TimeZoneId));

            var hasTradingStarted = existingSettings.MarketSchedule.Open.First() <= currentDay.TimeOfDay;
            
            // check holidays
            var newHolidays = newSettings.Holidays
                .Select(x => x.Date.Date)
                .Except(existingSettings.Holidays);
            var holidaysViolate = newHolidays.Contains(currentDay.Date) && hasTradingStarted;

            // check half-working days
            var newHalfWorkingDays =
                newSettings.MarketSchedule.HalfWorkingDays.Except(existingSettings.MarketSchedule.HalfWorkingDays);
            var halfWorkingDaysViolate =
                newHalfWorkingDays.Any(d => d.SameCalendarDay(currentDay)) && hasTradingStarted;

            if (holidaysViolate || halfWorkingDaysViolate)
            {
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.TradingDayAlreadyStarted);
            }

            return new Result<MarketSettingsErrorCodes>();
        }

        private static Result<MarketSettings, MarketSettingsErrorCodes> CreateMarketSettingsWithDefaults(
            MarketSettingsCreateOrUpdateDto model)
        {
            var marketSettings = MarketSettings.GetMarketSettingsWithDefaults(model);

            return marketSettings switch
            {
                InvalidMarketSettings invalid => invalid.ErrorCode,
                { } => marketSettings
            };
        }
    }
}