using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class CurrenciesService : ICurrenciesService
    {
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;

        public CurrenciesService(ICurrenciesRepository currenciesRepository, IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService)
        {
            _currenciesRepository = currenciesRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
        }

        public async Task<Result<CurrenciesErrorCodes>> InsertAsync(Currency currency, string username,
            string correlationId)
        {
            var result = await _currenciesRepository.InsertAsync(currency);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, currency.Id, AuditDataType.Currency,
                    currency.ToJson());

                await PublishCurrencyChangedEvent(null, currency, username, correlationId, ChangeType.Creation);
            }

            return result;
        }

        public async Task<Result<CurrenciesErrorCodes>> UpdateAsync(Currency currency, string username,
            string correlationId)
        {
            var existing = await _currenciesRepository.GetByIdAsync(currency.Id);

            if (existing.IsSuccess)
            {
                currency.Timestamp = existing.Value.Timestamp;
                var result = await _currenciesRepository.UpdateAsync(currency);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, currency.Id, AuditDataType.Currency,
                        currency.ToJson(), existing.Value.ToJson());
                    
                    await PublishCurrencyChangedEvent(existing.Value, currency, username, correlationId, ChangeType.Edition);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<CurrenciesErrorCodes>> DeleteAsync(string id, string username, string correlationId)
        {
            var existing = await _currenciesRepository.GetByIdAsync(id);

            if (existing.IsSuccess)
            {
                var result = await _currenciesRepository.DeleteAsync(id, existing.Value.Timestamp);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, id, AuditDataType.Currency,
                        oldStateJson: existing.Value.ToJson());
                    
                    await PublishCurrencyChangedEvent(existing.Value, null, username, correlationId, ChangeType.Deletion);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public Task<Result<Currency, CurrenciesErrorCodes>> GetByIdAsync(string mdsCode)
            => _currenciesRepository.GetByIdAsync(mdsCode);

        public Task<Result<List<Currency>, CurrenciesErrorCodes>> GetByPageAsync(int skip = default, int take = 20)
            => _currenciesRepository.GetByPageAsync(skip, take);

        public Task<Result<List<Currency>, CurrenciesErrorCodes>> GetAllAsync()
            => _currenciesRepository.GetAllAsync();

        private async Task PublishCurrencyChangedEvent
            (Currency oldCurrency, Currency newCurrency, string username, string correlationId, ChangeType changeType)
        {
            await _cqrsMessageSender.SendEvent(new CurrencyChangedEvent()
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldCurrency = _convertService.Convert<Currency, CurrencyContract>(oldCurrency),
                NewCurrency = _convertService.Convert<Currency, CurrencyContract>(newCurrency),
            });
        }
    }
}