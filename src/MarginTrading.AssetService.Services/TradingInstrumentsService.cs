using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class TradingInstrumentsService : ITradingInstrumentsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly DefaultTradingInstrumentSettings _defaultTradingInstrumentSettings;

        public TradingInstrumentsService(
            IProductsRepository productsRepository,
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IUnderlyingsCache underlyingsCache,
            DefaultTradingInstrumentSettings defaultTradingInstrumentSettings)
        {
            _productsRepository = productsRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _underlyingsCache = underlyingsCache;
            _defaultTradingInstrumentSettings = defaultTradingInstrumentSettings;
        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetAsync()
        {
            var allActiveAssetTypesIds = await _clientProfileSettingsRepository.GetActiveAssetTypeIdsForDefaultProfileAsync();
            var products = await _productsRepository.GetByAssetTypeIdsAsync(allActiveAssetTypesIds);
            return await GetTradingInstrumentsAsync(products);
        }

        public Task<IReadOnlyList<ITradingInstrument>> GetByTradingConditionAsync(string tradingConditionId)
        {
            return GetAsync();
        }

        public async Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null,
            int? skip = null, int? take = null)
        {
            skip ??= 0;
            take = PaginationHelper.GetTake(take);

            var allActiveAssetTypesIds = await _clientProfileSettingsRepository.GetActiveAssetTypeIdsForDefaultProfileAsync();
            var products = await _productsRepository.GetPagedByAssetTypeIdsAsync(allActiveAssetTypesIds, skip.Value, take.Value);

            var tradingInstruments = await GetTradingInstrumentsAsync(products.Contents);

            return new PaginatedResponse<ITradingInstrument>(tradingInstruments, products.Start, products.Size, products.TotalSize);
        }

        public async Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId)
        {
            var product = await _productsRepository.GetByIdAsync(assetPairId);

            if (product.IsFailed)
                return null;

            var isProductForAvailableAssetSettings =
                await _clientProfileSettingsRepository.IsAvailableForDefaultProfileAsync(product.Value.AssetType);

            if (!isProductForAvailableAssetSettings)
                return null;

            //tradingConditionId is not used because we always use default profile
            var tradingInstrument = await GetTradingInstrumentsAsync(new List<Product> { product.Value });

            return tradingInstrument.FirstOrDefault();
        }

        private async Task<IReadOnlyList<ITradingInstrument>> GetTradingInstrumentsAsync(IReadOnlyList<Product> products)
        {
            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
                return new List<ITradingInstrument>();

            var productAssetTypeIdMap = products.ToDictionary(x => x.ProductId, v => v.AssetType);

            var underlyings = products
                .Select(x => x.UnderlyingMdsCode)
                .Distinct()
                .Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var clientProfileSettings =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id,
                    productAssetTypeIdMap.Values))
                .ToDictionary(x => x.AssetTypeId, v => v);

            var result = new List<TradingInstrument>();
            foreach (var product in products)
            {
                var profileSettingsForProduct = clientProfileSettings[productAssetTypeIdMap[product.ProductId]];
                underlyings.TryGetValue(product.UnderlyingMdsCode, out var underlying);

                var tradingInstrument = TradingInstrument.CreateFromProduct(
                    product,
                    defaultProfile.Id,
                    profileSettingsForProduct.Margin,
                    underlying?.HedgeCost ?? _defaultTradingInstrumentSettings.HedgeCost,
                    underlying?.Spread ?? _defaultTradingInstrumentSettings.Spread);

                result.Add(tradingInstrument);
            }

            return result;
        }
    }
}