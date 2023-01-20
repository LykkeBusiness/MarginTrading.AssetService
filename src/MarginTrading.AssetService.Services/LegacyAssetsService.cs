using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;
using Asset = MarginTrading.AssetService.Contracts.LegacyAsset.Asset;
using Market = MarginTrading.AssetService.Contracts.LegacyAsset.Market;
using ClientProfile = MarginTrading.AssetService.Contracts.LegacyAsset.ClientProfile;
using TickFormula = MarginTrading.AssetService.Contracts.LegacyAsset.TickFormula;

namespace MarginTrading.AssetService.Services
{
    public class LegacyAssetsService : ILegacyAssetsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly IAssetTypesRepository _assetTypesRepository;
        private readonly ILogger<LegacyAssetsService> _logger;
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly string _brokerId;
        private readonly IList<string> _assetTypesWithZeroInterestRate;

        public LegacyAssetsService(
            IProductsRepository productsRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            ICurrenciesRepository currenciesRepository,
            ITickFormulaRepository tickFormulaRepository,
            IMarketSettingsRepository marketSettingsRepository,
            IProductCategoriesRepository productCategoriesRepository,
            IUnderlyingsCache underlyingsCache,
            IAssetTypesRepository assetTypesRepository,
            IBrokerSettingsApi brokerSettingsApi,
            string brokerId,
            IList<string> assetTypesWithZeroInterestRate,
            ILogger<LegacyAssetsService> logger)
        {
            _productsRepository = productsRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _currenciesRepository = currenciesRepository;
            _tickFormulaRepository = tickFormulaRepository;
            _marketSettingsRepository = marketSettingsRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _underlyingsCache = underlyingsCache;
            _assetTypesRepository = assetTypesRepository;
            _assetTypesWithZeroInterestRate = assetTypesWithZeroInterestRate;
            _logger = logger;
            _brokerSettingsApi = brokerSettingsApi;
            _brokerId = brokerId;
        }

        public async Task<List<Asset>> GetLegacyAssets(IEnumerable<string> productIds = null, bool startedOnly = true)
        {
            var products =
                (await _productsRepository.GetByProductsIdsAsync(productIds, startedOnly))
                .ToDictionary(x => x.ProductId, v => v);
            
            var brokerSettingsResponse =  await _brokerSettingsApi.GetByIdAsync(_brokerId);
            if (!(brokerSettingsResponse is { ErrorCode: BrokerSettingsErrorCodesContract.None }))
            {
                var message = brokerSettingsResponse == null
                    ? "Broker settings response is null"
                    : $"Broker settings response error code: {brokerSettingsResponse.ErrorCode} while getting broker settings by id: {_brokerId}";
                throw new InvalidOperationException(message);
            }

            var productTradingCurrencyMap = products.ToDictionary(x => x.Key, v => v.Value.TradingCurrency);
            var productMarketSettingsMap = products.ToDictionary(x => x.Key, v => v.Value.Market);
            var productTickFormulaMap = products.ToDictionary(x => x.Key, v => v.Value.TickFormula);
            var productToCategoryMap = products.ToDictionary(x => x.Key, v => v.Value.Category);
            var productAssetTypeIdMap = products.ToDictionary(x => x.Key, v => v.Value.AssetType);

            var tradingCurrencies =
                (await _currenciesRepository.GetByIdsAsync(productTradingCurrencyMap.Values.Distinct())).ToDictionary(x => x.Id, v => v);
            
            var availableClientProfileSettingsDict =
                (await _clientProfileSettingsRepository.GetAllAsync(string.Empty, productAssetTypeIdMap.Values.Distinct(), true))
                .GroupBy(x => x.AssetTypeId)
                .ToDictionary(x => x.Key, v => v.AsEnumerable());

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

            var assetTypes = (await _assetTypesRepository.GetAllAsync()).ToDictionary(x => x.Id, v=> v);

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
                    _logger.LogWarning("Missing underlying in cache for product with mdsCode:{MdsCode}", product.UnderlyingMdsCode);

                asset.SetAssetFieldsFromProduct(product);
                
                if(baseCurrency != null)
                    asset.SetAssetFieldsFromBaseCurrency(baseCurrency, _assetTypesWithZeroInterestRate);
                
                asset.SetAssetFieldsFromTradingCurrency(tradingCurrencies[productTradingCurrencyMap[id]], _assetTypesWithZeroInterestRate);

                if (availableClientProfileSettingsDict.TryGetValue(asset.Underlying.AssetType, out var clientProfileSettingsList))
                {
                    var availableClientProfileSettingsForAssetType = clientProfileSettingsList.Select(x =>
                        x.ToClientProfileWithRate(product.GetMarginRate(x.Margin)));
                    asset.Underlying.AvailableClientProfiles.AddRange(availableClientProfileSettingsForAssetType);
                }
                
                asset.SetAssetFieldsFromCategory(productCategories[productToCategoryMap[id]]);
                asset.SetAssetFieldsFromMarketSettings(productMarketSettings[productMarketSettingsMap[id]]);
                asset.SetAssetFieldsFromTickFormula(productTickFormulas[productTickFormulaMap[id]]);
                asset.SetAssetFieldsFromAssetType(assetTypes[productAssetTypeIdMap[id]]);
                asset.SetDividendFactorFields(productMarketSettings[productMarketSettingsMap[id]],
                    brokerSettingsResponse.BrokerSettings,
                    product);

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
                    MarketDetails = new Market
                    {
                        Calendar = new Calendar(),
                        MarketHours = new MarketHours(),
                        DividendsFactor = new DividendsFactor()
                    },
                    InterestRates = new List<InterestRate>(),
                    DividendsFactor = new DividendsFactor(),
                    AvailableClientProfiles = new List<ClientProfile>()
                }
            };
            return asset;
        }
    }
}