// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Extensions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

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

        public async Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettingsCreateOrUpdateDto model,
            string username)
        {
            var marketSettings = MarketSettingsFactory.FromRequest(model);
            
            if (marketSettings is InvalidMarketSettings invalid)
                return invalid.ErrorCode;
            
            var validationResult = marketSettings.Validate();
            
            if (validationResult.IsFailed)
                return validationResult;
            
            var addResult = await _marketSettingsRepository.AddAsync(marketSettings);
            
            if (addResult.IsFailed)
                return addResult;
            
            await _auditService.CreateAuditRecord(AuditEventType.Creation, username, marketSettings);
            
            await PublishMarketSettingsChangedEvent(null, marketSettings, username, ChangeType.Creation);
            
            return new Result<MarketSettingsErrorCodes>();
        }

        public async Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettingsCreateOrUpdateDto model, string username)
        {
            var marketSettings = MarketSettingsFactory.FromRequest(model);
            
            if (marketSettings is InvalidMarketSettings invalid)
                return invalid.ErrorCode;
            
            var currentSettings = await _marketSettingsRepository.GetByIdAsync(marketSettings.Id);

            if (currentSettings == null)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.MarketSettingsDoNotExist);
            
            var validationResult = marketSettings.Validate();
            if (validationResult.IsFailed)
                return validationResult;

            var isUpdateValid =
                new MarketSettingsUpdateValidator(currentSettings, marketSettings).ValidAt(DateTime.UtcNow);
            if (!isUpdateValid)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.TradingDayAlreadyStarted);

            var updateResult = await _marketSettingsRepository.UpdateAsync(marketSettings);

            if (updateResult.IsFailed)
                return updateResult;

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, marketSettings, currentSettings);

            await PublishMarketSettingsChangedEvent(currentSettings, marketSettings, username, ChangeType.Edition);

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
    }
}