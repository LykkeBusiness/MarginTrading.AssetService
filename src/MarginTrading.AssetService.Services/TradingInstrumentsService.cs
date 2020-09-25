using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class TradingInstrumentsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IUnderlyingsCache _underlyingsCache;

        public TradingInstrumentsService(
            IProductsRepository productsRepository,
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IUnderlyingsCache underlyingsCache)
        {
            _productsRepository = productsRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _underlyingsCache = underlyingsCache;
        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetAsync()
        {
            var products = await _productsRepository.GetAllAsync();
            return await GetTradingInstrumentsAsync(products.Value);
        }

        private async Task<IReadOnlyList<ITradingInstrument>> GetTradingInstrumentsAsync(IReadOnlyList<Product> products)
        {
            //TODO:What to do with missing product or default profile
            var defaultProfile = await _clientProfilesRepository.GetDefaultAsync();
            if (defaultProfile == null)
                return null;

            var productAssetTypeIdMap = products.ToDictionary(x => x.ProductId, v => v.AssetType);

            var underlyings = products.Select(x => x.UnderlyingMdsCode).Distinct().Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var clientProfileSettings =
                (await _clientProfileSettingsRepository.GetAllByProfileAndMultipleAssetTypesAsync(defaultProfile.Id,
                    productAssetTypeIdMap.Values))
                .ToDictionary(x => x.AssetTypeId, v => v);

            var result = new List<TradingInstrument>();
            foreach (var product in products)
            {
                var profileSettingsForProduct = clientProfileSettings[productAssetTypeIdMap[product.ProductId]];
                var underlying = underlyings[product.UnderlyingMdsCode];

                var tradingInstrument = TradingInstrument.CreateFromProduct(product, defaultProfile.Id,
                    profileSettingsForProduct.Margin, underlying.HedgeCost, underlying.Spread);

                result.Add(tradingInstrument);
            }

            return result;
        }

        public Task<IReadOnlyList<ITradingInstrument>> GetByTradingConditionAsync(string tradingConditionId)
        {
            return GetAsync();
            //TODO:Ask if I need to filter by something or always return for default
        }

        public async Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null,
            int? skip = null, int? take = null)
        {
            skip ??= 0;
            take = PaginationHelper.GetTake(take);

            //TODO:Ask if the pagination is correct
            var products = (await _productsRepository.GetPagedAsync(skip.Value, take.Value));

            var tradingInstruments = await GetTradingInstrumentsAsync(products.Contents);

            return new PaginatedResponse<ITradingInstrument>(tradingInstruments, products.Start, products.Size, products.TotalSize);
        }

        public async Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId)
        {
            var product = await _productsRepository.GetByIdAsync(assetPairId);

            if (product.IsFailed)
                return null;

            var tradingInstrument = await GetTradingInstrumentsAsync(new List<Product> { product.Value });

            return tradingInstrument.FirstOrDefault();
        }
    }
}