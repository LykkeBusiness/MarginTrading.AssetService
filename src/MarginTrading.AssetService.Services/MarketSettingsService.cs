// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Extensions;
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

        public MarketSettingsService(
            IMarketSettingsRepository marketSettingsRepository,
            IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService)
        {
            _marketSettingsRepository = marketSettingsRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
        }

        public Task<MarketSettings> GetByIdAsync(string id)
            => _marketSettingsRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<MarketSettings>> GetAllMarketSettingsAsync()
            => _marketSettingsRepository.GetAllMarketSettingsAsync();

        public async Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettingsCreateOrUpdateDto model, string username, string correlationId)
        {
            var marketSettings = MarketSettings.GetMarketSettingsWithDefaults(model);

            var validationResult = ValidateSettings(marketSettings);

            if (validationResult.IsFailed)
                return validationResult;

            var addResult = await _marketSettingsRepository.AddAsync(marketSettings);

            if (addResult.IsFailed)
                return addResult;

            await _auditService.TryAudit(correlationId, username, model.Id, AuditDataType.MarketSettings,
                marketSettings.ToJson());

            await PublishMarketSettingsChangedEvent(null, marketSettings, username, correlationId, ChangeType.Creation);

            return new Result<MarketSettingsErrorCodes>();
        }

        public async Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettingsCreateOrUpdateDto model, string username, string correlationId)
        {
            var marketSettings = MarketSettings.GetMarketSettingsWithDefaults(model);

            var currentSettings = await _marketSettingsRepository.GetByIdAsync(marketSettings.Id);

            if (currentSettings == null)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);

            var validationResult = ValidateSettings(marketSettings, currentSettings);

            if (validationResult.IsFailed)
                return validationResult;

            var updateResult = await _marketSettingsRepository.UpdateAsync(marketSettings);

            if (updateResult.IsFailed)
                return updateResult;

            await _auditService.TryAudit(correlationId, username, marketSettings.Id, AuditDataType.MarketSettings,
                marketSettings.ToJson(), currentSettings.ToJson());

            await PublishMarketSettingsChangedEvent(currentSettings, marketSettings, username, correlationId, ChangeType.Edition);

            return new Result<MarketSettingsErrorCodes>();
        }

        public async Task<Result<MarketSettingsErrorCodes>> DeleteAsync(string id, string username, string correlationId)
        {
            var existing = await _marketSettingsRepository.GetByIdAsync(id);

            if (existing == null)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);
            
            if(await _marketSettingsRepository.MarketSettingsAssignedToAnyProductAsync(id))
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.CannotDeleteMarketSettingsAssignedToAnyProduct);

            var deleteResult = await _marketSettingsRepository.DeleteAsync(id);

            if (deleteResult.IsFailed)
                return deleteResult;

            await _auditService.TryAudit(correlationId, username, id, AuditDataType.MarketSettings,
                oldStateJson: existing.ToJson());

            await PublishMarketSettingsChangedEvent(existing, null, username, correlationId, ChangeType.Deletion);

            return new Result<MarketSettingsErrorCodes>();
        }

        private Result<MarketSettingsErrorCodes> ValidateSettings(MarketSettings model, MarketSettings currentMarketSettings = null)
        {
            var valid = TZConvert.TryGetTimeZoneInfo(model.Timezone, out var timezone);
            if (!valid)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidTimezone);

            if (model.Open.TotalHours >= 24 || model.Close.TotalHours >= 24 || (model.Open > model.Close && model.Close != TimeSpan.Zero))
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidOpenAndCloseHours);

            var openUtc = model.Open.ShiftToUtc(timezone);
            var closeUtc = model.Close.ShiftToUtc(timezone);

            if (openUtc.TotalHours >= 24 || closeUtc.TotalHours >= 24 ||
                openUtc.TotalHours < 0 || closeUtc.TotalHours < 0)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.OpenAndCloseWithAppliedTimezoneMustBeInTheSameDay);

            if (model.DividendsLong < 0 || model.DividendsLong > 100)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsLongValue);

            if (model.DividendsShort < 0 || model.DividendsShort > 100)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsShortValue);

            if (model.Dividends871M < 0 || model.Dividends871M > 100)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividends871MValue);

            if (currentMarketSettings == null) 
                return new Result<MarketSettingsErrorCodes>();

            //This is the current day taking into account the timezone
            var currentDay = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZConvert.GetTimeZoneInfo(currentMarketSettings.Timezone));
            var newHolidays = model.Holidays.Select(x => x.Date.Date).Except(currentMarketSettings.Holidays);

            //Validate if we try to add holiday for already started trading day
            if (newHolidays.Contains(currentDay.Date) && currentMarketSettings.Open <= currentDay.TimeOfDay &&
                //Close will be Zero when it is set to 00h next day
                (currentMarketSettings.Close >= currentDay.TimeOfDay || model.Close == TimeSpan.Zero))
            {
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.TradingDayAlreadyStarted);
            }

            return new Result<MarketSettingsErrorCodes>();
        }

        private async Task PublishMarketSettingsChangedEvent
            (MarketSettings oldSettings, MarketSettings newSettings, string username, string correlationId, ChangeType changeType)
        {
            await _cqrsMessageSender.SendEvent(new MarketSettingsChangedEvent
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldMarketSettings = _convertService.Convert<MarketSettings, MarketSettingsContract>(oldSettings),
                NewMarketSettings = _convertService.Convert<MarketSettings, MarketSettingsContract>(newSettings),
            });
        }
    }
}