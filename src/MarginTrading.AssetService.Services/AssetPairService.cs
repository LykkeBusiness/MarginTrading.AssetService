using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class AssetPairService : IAssetPairService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;

        public AssetPairService(IProductsRepository productsRepository, DefaultLegalEntitySettings defaultLegalEntitySettings)
        {
            _productsRepository = productsRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
        }

        public async Task<IAssetPair> GetByIdAsync(string assetPairId)
        {
            var result = await _productsRepository.GetByIdAsync(assetPairId);

            return result.IsSuccess ? AssetPair.CreateFromProduct(result.Value, _defaultLegalEntitySettings.DefaultLegalEntity) : null;
        }

        public async Task<PaginatedResponse<IAssetPair>> GetPaginatedWithFilterAsync(string filter, int? skip, int? take)
        {
            skip ??= 0;
            take = PaginationHelper.GetTake(take);
            var products = await _productsRepository.GetPagedWithFilterAsync(filter, skip.Value, take.Value);

            var assetPairs = products.Contents
                .Select(x => AssetPair.CreateFromProduct(x, _defaultLegalEntitySettings.DefaultLegalEntity)).ToList();

            return new PaginatedResponse<IAssetPair>(assetPairs,products.Start, products.Size, products.TotalSize);
        }

        public async Task<IReadOnlyList<IAssetPair>> GetWithFilterAsync(string filter)
        {
            var products = await _productsRepository.GetWithFilterAsync(filter);

            var assetPairs = products
                .Select(x => AssetPair.CreateFromProduct(x, _defaultLegalEntitySettings.DefaultLegalEntity)).ToList();

            return assetPairs;
        }
    }
}