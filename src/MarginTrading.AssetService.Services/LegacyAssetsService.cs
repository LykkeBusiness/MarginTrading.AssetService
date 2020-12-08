using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Cronut.Dto.Assets;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Asset = Cronut.Dto.Assets.Asset;
using Market = Cronut.Dto.Assets.Market;

namespace MarginTrading.AssetService.Services
{
    public class LegacyAssetsService : ILegacyAssetsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILog _log;

        public LegacyAssetsService(
            IProductsRepository productsRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IClientProfilesRepository clientProfilesRepository,
            ICurrenciesRepository currenciesRepository,
            ITickFormulaRepository tickFormulaRepository,
            IMarketSettingsRepository marketSettingsRepository,
            IProductCategoriesRepository productCategoriesRepository,
            IUnderlyingsCache underlyingsCache,
            ILog log)
        {
            _productsRepository = productsRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _currenciesRepository = currenciesRepository;
            _tickFormulaRepository = tickFormulaRepository;
            _marketSettingsRepository = marketSettingsRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _underlyingsCache = underlyingsCache;
            _log = log;
        }

        public async Task<List<Asset>> GetLegacyAssets(IEnumerable<string> productIds = null)
        {
            var products =
                (await _productsRepository.GetByProductsIdsAsync(productIds))
                .Where(x => x.IsStarted)
                .ToDictionary(x => x.ProductId, v => v);

            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
                throw new InvalidOperationException("There is not default client profile in the system");

            var productTradingCurrencyMap = products.ToDictionary(x => x.Key, v => v.Value.TradingCurrency);
            var productMarketSettingsMap = products.ToDictionary(x => x.Key, v => v.Value.Market);
            var productTickFormulaMap = products.ToDictionary(x => x.Key, v => v.Value.TickFormula);
            var productToCategoryMap = products.ToDictionary(x => x.Key, v => v.Value.Category);
            var productAssetTypeIdMap = products.ToDictionary(x => x.Key, v => v.Value.AssetType);

            var tradingCurrencies =
                (await _currenciesRepository.GetByIdsAsync(productTradingCurrencyMap.Values.Distinct())).ToDictionary(x => x.Id, v => v);

            var clientProfileSettings =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id,
                    productAssetTypeIdMap.Values.Distinct()))
                .ToDictionary(x => x.AssetTypeId, v => v);

            var underlyings = products.Select(x => x.Value.UnderlyingMdsCode).Distinct()
                .Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var baseCurrenciesIds = underlyings.Values.Where(x => !string.IsNullOrEmpty(x.BaseCurrency))
                .Select(x => x.BaseCurrency).Distinct();

            var baseCurrencies =
                (await _currenciesRepository.GetByIdsAsync(baseCurrenciesIds)).ToDictionary(x => x.Id, v => v);

            var productCategories =
                (await _productCategoriesRepository.GetByIdsAsync(productToCategoryMap.Values.Distinct())).ToDictionary(x => x.Id, v => v);

            var productMarketSettings =
                (await _marketSettingsRepository.GetByIdsAsync(productMarketSettingsMap.Values.Distinct())).ToDictionary(x => x.Id, v => v);

            var productTickFormulas =
                (await _tickFormulaRepository.GetByIdsAsync(productTickFormulaMap.Values.Distinct())).ToDictionary(x => x.Id, v => v);

            var result = new List<Asset>();
            foreach (var product in products.Values)
            {
                var id = product.ProductId;
                var asset = CreateEmptyAsset();

                underlyings.TryGetValue(product.UnderlyingMdsCode, out var underlying);
                var baseCurrencyId = underlying?.BaseCurrency;
                var baseCurrency = string.IsNullOrEmpty(baseCurrencyId) ? null :
                    baseCurrencies.ContainsKey(baseCurrencyId) ? baseCurrencies[baseCurrencyId] : null;

                if (underlying != null)
                    asset.SetAssetFieldsFromUnderlying(underlying);
                else
                    _log.WriteWarning(nameof(LegacyAssetsService), nameof(GetLegacyAssets),
                        $"Missing underlying in cache for product with mdsCode:{product.UnderlyingMdsCode}");

                if(baseCurrency != null)
                    asset.SetAssetFieldsFromBaseCurrency(baseCurrency);

                asset.SetAssetFieldsFromProduct(product);
                asset.SetAssetFieldsFromTradingCurrency(tradingCurrencies[productTradingCurrencyMap[id]]);
                asset.SetAssetFieldsFromClientProfileSettings(clientProfileSettings[productAssetTypeIdMap[id]]);
                asset.SetAssetFieldsFromCategory(productCategories[productToCategoryMap[id]]);
                asset.SetAssetFieldsFromMarketSettings(productMarketSettings[productMarketSettingsMap[id]]);
                asset.SetAssetFieldsFromTickFormula(productTickFormulas[productTickFormulaMap[id]]);

                result.Add(asset);
            }

            return result;
        }

        private static Asset CreateEmptyAsset()
        {
            var asset = new Asset
            {
                DividendsFactor = new DividendsFactor(),
                TickFormulaDetails = new TickFormula
                {
                    TickFormulaParameters = new TickFormulaParameters()
                },
                Underlying = new Underlying
                {
                    ExecutionFeeParameter = new ExecutionFeeParameter(),
                    MarketDetails = new Market
                    {
                        Calendar = new Calendar(),
                        MarketHours = new MarketHours(),
                        DividendsFactor = new DividendsFactor(),
                    },
                    InterestRates = new List<InterestRate>(),
                    DividendsFactor = new DividendsFactor(),
                },
            };
            return asset;
        }
    }
}