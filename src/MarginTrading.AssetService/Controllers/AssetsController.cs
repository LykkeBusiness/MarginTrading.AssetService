// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asset = MarginTrading.AssetService.Contracts.LegacyAsset.Asset;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Assets management
    /// </summary>
    [Authorize]
    [Route("api/assets")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class AssetsController : Controller, IAssetsApi
    {
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly IAssetsRepository _assetsRepository;
        private readonly IConvertService _convertService;
        
        public AssetsController(
            ILegacyAssetsCache legacyAssetsCache,
            IAssetsRepository assetsRepository,
            IConvertService convertService)
        {
            _legacyAssetsCache = legacyAssetsCache;
            _assetsRepository = assetsRepository;
            _convertService = convertService;
        }

        /// <summary>
        /// Get the list of assets
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<AssetContract>> List()
        {
            var data = await _assetsRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<IAsset, AssetContract>(x)).ToList();
        }

        /// <summary>
        /// Get deleted product ids
        /// </summary>
        [HttpGet]
        [Route("discontinued-ids")]
        public async Task<List<string>> GetDiscontinuedIds()
        {
            var data = await _assetsRepository.GetDiscontinuedIdsAsync();

            return data.ToList();
        }
        
        /// <summary>
        /// Get the list of assets with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<AssetContract>> ListByPages(
            [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _assetsRepository.GetByPagesAsync(skip, take);
            
            return new PaginatedResponseContract<AssetContract>(
                contents: data.Contents.Select(x => _convertService.Convert<IAsset, AssetContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Get the asset
        /// </summary>
        [HttpGet]
        [Route("{assetId}")]
        public async Task<AssetContract> Get(string assetId)
        {
            var obj = await _assetsRepository.GetAsync(assetId);
            
            return _convertService.Convert<IAsset, AssetContract>(obj);
        }
        
        /// <summary>
        /// Returns duplicates for a given set of product isins (short & long)
        /// </summary>
        [HttpPost]
        [Route("duplicated-isins")]
        public async Task<List<string>> GetDuplicatedIsins([FromBody] string[] isins)
        {
            var result = await _assetsRepository.GetDuplicatedIsinsAsync(isins);

            return result.ToList();
        }

        /// <summary>
        /// Get the list of legacy assets
        /// </summary>
        [HttpGet]
        [Route("legacy")]
        public Task<List<Asset>> GetLegacyAssets()
        {
            var result = _legacyAssetsCache.GetAll();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Get legacy asset by id
        /// </summary>
        [HttpGet]
        [Route("legacy/{assetId}")]
        public Task<Asset> GetLegacyAssetById(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            var result = _legacyAssetsCache.GetById(assetId);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Performs search and returns list of asset ids
        /// </summary>
        [HttpPost("/api/assets/legacy/search")]
        public Task<IEnumerable<Asset>> SearchLegacyAssets([FromBody]SearchLegacyAssetsRequest request)
        {
            return Task.FromResult(_legacyAssetsCache.Search(request));
        }
    }
}