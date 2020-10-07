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
using MarginTrading.AssetService.Core.Settings.Rates;
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
        private readonly DefaultRateSettings _defaultRateSettings;
        private readonly ILog _log;

        public RateSettingsService(
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IProductsRepository productsRepository,
            IUnderlyingsCache underlyingsCache,
            ICurrenciesRepository currenciesRepository,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            DefaultRateSettings defaultRateSettings,
            ILog log)
        {
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _productsRepository = productsRepository;
            _underlyingsCache = underlyingsCache;
            _currenciesRepository = currenciesRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
            _defaultRateSettings = defaultRateSettings;
            _log = log;
        }

        #region Order Execution

        public async Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRatesAsync(IList<string> assetPairIds = null)
        {
            var productAssetTypeMap = await _productsRepository.GetProductAssetTypeMapAsync(assetPairIds);

            //If filter is empty we should get all products(asset pairs)
            if (assetPairIds == null || !assetPairIds.Any())
                assetPairIds = productAssetTypeMap.Keys.ToList();

            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
            {
                _log.WriteWarning(nameof(RateSettingsService), nameof(GetOrderExecutionRatesAsync),
                    "Missing default client profile, default values will be used to create OrderExecutionRates");

                return assetPairIds.Select(x =>
                        OrderExecutionRate.FromDefault(_defaultRateSettings.DefaultOrderExecutionSettings, x))
                    .ToList();
            }

            var clientProfileSettingsMap =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id,
                    productAssetTypeMap.Values)).ToDictionary(x => x.AssetTypeId, v => v);

            var result = new List<OrderExecutionRate>();
            foreach (var assetPairId in assetPairIds)
            {
                var containsAssetType = productAssetTypeMap.TryGetValue(assetPairId, out var assetTypeId);

                if (!containsAssetType || clientProfileSettingsMap.ContainsKey(assetTypeId))
                {
                    _log.WriteWarning(nameof(RateSettingsService), nameof(GetOrderExecutionRatesAsync),
                        $"Missing product with id: {assetPairId} , default values will be used to create OrderExecutionRates");

                    result.Add(OrderExecutionRate.FromDefault(_defaultRateSettings.DefaultOrderExecutionSettings, assetPairId));
                    continue;
                }

                var clientProfileSettings = clientProfileSettingsMap[assetTypeId];
                result.Add(OrderExecutionRate.Create(assetPairId,
                    clientProfileSettings.ExecutionFeesCap,
                    clientProfileSettings.ExecutionFeesFloor,
                    clientProfileSettings.ExecutionFeesRate,
                    _defaultLegalEntitySettings.DefaultLegalEntity));
            }

            return result;
        }

        #endregion Order Execution

        #region Overnight Swaps

        public async Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync(IList<string> assetPairIds = null)
        {
            var products = (await _productsRepository.GetByProductsIdsAsync(assetPairIds)).ToDictionary(x => x.ProductId, v=> v);

            //If filter is empty we should get all products(asset pairs)
            if (assetPairIds == null || !assetPairIds.Any())
                assetPairIds = products.Keys.ToList();

            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
            {
                _log.WriteWarning(nameof(RateSettingsService), nameof(GetOvernightSwapRatesAsync),
                    "Missing default client profile, default values will be used to create OvernightSwapRate");

                return assetPairIds.Select(x =>
                        OvernightSwapRate.FromDefault(_defaultRateSettings.DefaultOvernightSwapSettings, x))
                    .ToList();
            }
            var productTradingCurrencyMap = products.ToDictionary(x => x.Key, v => v.Value.TradingCurrency);

            var tradingCurrencies =
                (await _currenciesRepository.GetByIdsAsync(productTradingCurrencyMap.Values)).ToDictionary(x => x.Id, v => v);

            var productAssetTypeIdMap = products.ToDictionary(x => x.Key, v => v.Value.AssetType);

            var clientProfileSettings =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id, productAssetTypeIdMap.Values))
                .ToDictionary(x => x.AssetTypeId, v => v);

            var underlyings = products.Select(x => x.Value.UnderlyingMdsCode).Distinct()
                .Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var baseCurrenciesIds = underlyings.Values.Where(x => !string.IsNullOrEmpty(x.BaseCurrency))
                .Select(x => x.BaseCurrency).Distinct();

            var baseCurrencies =
                (await _currenciesRepository.GetByIdsAsync(baseCurrenciesIds)).ToDictionary(x => x.Id, v => v);

            var result = new List<OvernightSwapRate>();
            foreach (var assetPairId in assetPairIds)
            {
                products.TryGetValue(assetPairId, out var product);
                if (product == null)
                {
                    _log.WriteWarning(nameof(RateSettingsService), nameof(GetOvernightSwapRatesAsync),
                        $"Missing product with id: {assetPairId} , default values will be used to create OvernightSwapRate");

                    result.Add(OvernightSwapRate.FromDefault(_defaultRateSettings.DefaultOvernightSwapSettings, assetPairId));
                    continue;
                }

                var productId = product.ProductId;
                underlyings.TryGetValue(product.UnderlyingMdsCode, out var underlying);
                var baseCurrencyId = underlying?.BaseCurrency;
                var baseCurrency = string.IsNullOrEmpty(baseCurrencyId) ? null :
                    baseCurrencies.ContainsKey(baseCurrencyId) ? baseCurrencies[baseCurrencyId] : null;
                var tradingCurrencyId = productTradingCurrencyMap[productId];
                var tradingCurrency = tradingCurrencies[tradingCurrencyId];
                var assetTypeId = productAssetTypeIdMap[productId];
                var profileSettings = clientProfileSettings[assetTypeId];

                var rate = new OvernightSwapRate
                {
                    AssetPairId = productId,
                    VariableRateQuote = tradingCurrency.InterestRateMdsCode,
                    FixRate = profileSettings.FinancingFeesRate,
                    RepoSurchargePercent = underlying?.RepoSurchargePercent ?? _defaultRateSettings.DefaultOvernightSwapSettings.RepoSurchargePercent,
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
            return OnBehalfRate.FromDefault(_defaultRateSettings.DefaultOnBehalfSettings);
        }

        #endregion On Behalf
    }
}