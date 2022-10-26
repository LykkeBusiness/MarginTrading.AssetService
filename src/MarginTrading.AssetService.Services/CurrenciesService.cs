using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.Enums;
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
        private readonly CorrelationContextAccessor _correlationContextAccessor;

        public CurrenciesService(ICurrenciesRepository currenciesRepository, IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService,
            CorrelationContextAccessor correlationContextAccessor)
        {
            _currenciesRepository = currenciesRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task<Result<CurrenciesErrorCodes>> InsertAsync(Currency currency, string username)
        {
            var result = await _currenciesRepository.InsertAsync(currency);

            if (result.IsSuccess)
            {
                await _auditService.CreateAuditRecord(AuditEventType.Creation, username, currency);

                await PublishCurrencyChangedEvent(null, currency, username, ChangeType.Creation);
            }

            return result;
        }

        public async Task<Result<CurrenciesErrorCodes>> UpdateAsync(Currency currency, string username)
        {
            var existing = await _currenciesRepository.GetByIdAsync(currency.Id);

            if (existing.IsSuccess)
            {
                currency.Timestamp = existing.Value.Timestamp;
                var result = await _currenciesRepository.UpdateAsync(currency);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Edition, username, currency, existing.Value);
                    
                    await PublishCurrencyChangedEvent(existing.Value, currency, username, ChangeType.Edition);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<CurrenciesErrorCodes>> DeleteAsync(string id, string username)
        {
            var existing = await _currenciesRepository.GetByIdAsync(id);

            if (existing.IsSuccess)
            {
                var productsExist = await _currenciesRepository.CurrencyHasProductsAsync(id);
                if(productsExist) return new Result<CurrenciesErrorCodes>(CurrenciesErrorCodes.CannotDeleteCurrencyWithAttachedProducts);
                
                var result = await _currenciesRepository.DeleteAsync(id, existing.Value.Timestamp);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, existing.Value);
                    
                    await PublishCurrencyChangedEvent(existing.Value, null, username, ChangeType.Deletion);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public Task<Result<Currency, CurrenciesErrorCodes>> GetByIdAsync(string id)
            => _currenciesRepository.GetByIdAsync(id);

        public Task<Result<List<Currency>, CurrenciesErrorCodes>> GetByPageAsync(int skip = default, int take = 20)
            => _currenciesRepository.GetByPageAsync(skip, take);

        public Task<Result<List<Currency>, CurrenciesErrorCodes>> GetAllAsync()
            => _currenciesRepository.GetAllAsync();

        private async Task PublishCurrencyChangedEvent(Currency oldCurrency, Currency newCurrency, string username, ChangeType changeType)
        {
            await _cqrsMessageSender.SendEvent(new CurrencyChangedEvent
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = _correlationContextAccessor.GetOrGenerateCorrelationId(),
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<Currency, CurrencyContract>(oldCurrency),
                NewValue = _convertService.Convert<Currency, CurrencyContract>(newCurrency)
            });
        }
    }
}