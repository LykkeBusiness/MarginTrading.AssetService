// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain.Rates;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Services
{
    public class RateSettingsService : IRateSettingsService
    {
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        private readonly ILog _log;

        public RateSettingsService(
            IClientProfilesRepository clientProfilesRepository, 
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IProductsRepository productsRepository,
            IUnderlyingsCache underlyingsCache,
            ICurrenciesRepository currenciesRepository,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            ILog log)
        {
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _productsRepository = productsRepository;
            _underlyingsCache = underlyingsCache;
            _currenciesRepository = currenciesRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
            _log = log;
        }

        #region Order Execution

        public async Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRatesAsync(IList<string> assetPairIds = null)
        {
            //TODO:What to do with missing product or default profile
            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
                return null;

            //TODO: Maybe check we found products for all asset pair ids
            var assetTypeProductIdMap = await _productsRepository.GetAssetTypesByProductsIdsAsync(assetPairIds);

            var clientProfileSettings =
                await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id,
                    assetTypeProductIdMap.Keys);

            var result = clientProfileSettings.Select(x => OrderExecutionRate.Create(assetTypeProductIdMap[x.AssetTypeId],
                x.ExecutionFeesCap, x.ExecutionFeesFloor, x.FinancingFeesRate,
                _defaultLegalEntitySettings.DefaultLegalEntity)).ToList();

            return result;
        }

        #endregion Order Execution

        #region Overnight Swaps

        public async Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync(IList<string> assetPairIds = null)
        {
            //TODO:What to do with missing product or default profile
            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
                return null;

            var result = new List<OvernightSwapRate>();

            //TODO: Maybe check we found products for all asset pair ids
            var products = await _productsRepository.GetByProductsIdsAsync(assetPairIds);

            var productTradingCurrencyMap = products.ToDictionary(x => x.ProductId, v => v.TradingCurrency);

            var tradingCurrencies =
                (await _currenciesRepository.GetByIdsAsync(productTradingCurrencyMap.Values)).ToDictionary(x => x.Id, v => v);

            var productAssetTypeIdMap = products.ToDictionary(x => x.ProductId, v => v.AssetType);

            var clientProfileSettings =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id, productAssetTypeIdMap.Values))
                .ToDictionary(x => x.AssetTypeId, v=>v);

            var underlyings = products.Select(x => x.UnderlyingMdsCode).Distinct().Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var baseCurrenciesIds = underlyings.Values.Where(x => !string.IsNullOrEmpty(x.BaseCurrency))
                .Select(x => x.BaseCurrency).Distinct();

            var baseCurrencies =
                (await _currenciesRepository.GetByIdsAsync(baseCurrenciesIds)).ToDictionary(x => x.Id, v => v);

            foreach (var product in products)
            {
                var baseCurrencyId = underlyings[product.UnderlyingMdsCode].BaseCurrency;
                var baseCurrency = string.IsNullOrEmpty(baseCurrencyId) ? null :
                    baseCurrencies.ContainsKey(baseCurrencyId) ? baseCurrencies[baseCurrencyId] : null;

                var rate = new OvernightSwapRate
                {
                    AssetPairId = product.ProductId,
                    VariableRateQuote = tradingCurrencies[productTradingCurrencyMap[product.ProductId]].InterestRateMdsCode,
                    FixRate = clientProfileSettings[productAssetTypeIdMap[product.ProductId]].FinancingFeesRate,
                    RepoSurchargePercent = underlyings[product.UnderlyingMdsCode].RepoSurchargePercent,
                    VariableRateBase = baseCurrency?.InterestRateMdsCode,
                };

                result.Add(rate);
            }

            return result;
        }

        #endregion Overnight Swaps

        #region On Behalf

        public async Task<OnBehalfRate> GetOnBehalfRateAsync()
        {
            //Changes will be made in ComissionsService, this is just a mock
            return new OnBehalfRate
            {
                CommissionAsset = "EUR",
                LegalEntity = _defaultLegalEntitySettings.DefaultLegalEntity,
                Commission = 0,
            };
        }

        #endregion On Behalf
    }
}