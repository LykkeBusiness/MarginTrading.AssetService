﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
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
        private readonly ILog _log;

        public AssetPairService(
            IProductsRepository productsRepository,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            ILog log)
        {
            _productsRepository = productsRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
            _log = log;
        }
        public async Task<IAssetPair> GetByIdAsync(string assetPairId)
        {
            var result = await _productsRepository.GetByIdAsync(assetPairId);

            return result.IsSuccess ? AssetPair.CreateFromProduct(result.Value, _defaultLegalEntitySettings.DefaultLegalEntity) : null;
        }

        public async Task<IReadOnlyList<IAssetPair>> GetByIdsAsync(IEnumerable<string> assetPairIds)
        {
            var products = await _productsRepository.GetByProductsIdsAsync(assetPairIds);
            var result = products
                .Select(x => AssetPair.CreateFromProduct(x, _defaultLegalEntitySettings.DefaultLegalEntity)).ToList();

            return result;
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

        public async Task<IAssetPair> ChangeSuspendStatusAsync(string assetPairId, bool status)
        {
            var result = await _productsRepository.ChangeSuspendFlagAsync(assetPairId, status);

            if (result.IsFailed)
            {
                _log.WriteError(nameof(AssetPairService), $"Could not change product suspended flag because product with id :{assetPairId} does not exist");
                return null;
            }

            return AssetPair.CreateFromProduct(result.Value, _defaultLegalEntitySettings.DefaultLegalEntity);
        }
    }
}