using System;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;

namespace MarginTrading.AssetService.Services
{
    public class SettlementCurrencyService : ISettlementCurrencyService
    {
        private string _settlementCurrency;

        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly string _brokerId;

        public SettlementCurrencyService(IBrokerSettingsApi brokerSettingsApi, string brokerId)
        {
            _brokerSettingsApi = brokerSettingsApi;
            _brokerId = brokerId;
        }

        public async Task<string> GetSettlementCurrencyAsync()
        {
            if (!string.IsNullOrEmpty(_settlementCurrency))
                return _settlementCurrency;

            //Since settlement currency is readonly and won't be updated I don't think sync is needed
            _settlementCurrency = await _brokerSettingsApi.GetSettlementCurrencyOrThrowAsync(_brokerId);

            return _settlementCurrency;
        }
    }
}