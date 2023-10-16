// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.Contracts.Responses;
using Lykke.Snow.Common;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Requests;

using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MoreLinq;

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
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly IUnderlyingsApi _underlyingsApi;
        private readonly IMarginTradingBlobRepository _temp;
        
        public AssetsController(
            ILegacyAssetsCache legacyAssetsCache,
            IAssetsRepository assetsRepository,
            IConvertService convertService,
            ILegacyAssetsService legacyAssetsService,
            IUnderlyingsApi underlyingsApi,
            IMarginTradingBlobRepository temp)
        {
            _legacyAssetsCache = legacyAssetsCache;
            _assetsRepository = assetsRepository;
            _convertService = convertService;
            _legacyAssetsService = legacyAssetsService;
            _underlyingsApi = underlyingsApi;
            _temp = temp;
        }
        
        /// <summary>
        /// Temp api (to be deleted)
        /// </summary>
        [HttpPost("migrate-871m-warning")]
        public async Task Migrage871mWarning()
        {
            var underlyings = await _underlyingsApi.GetAllAsync(new GetUnderlyingsRequestV2
            {
                Take = 0
            });

            var mdsCodes = underlyings.Underlyings
                .Where(x => x.Eligible871M)
                .Select(x => x.MdsCode)
                .ToList();

            foreach (var batch in mdsCodes.Batch(100))
            {
                await _temp.TempFor871mMigration(batch.ToList());
            }
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
        /// Returns duplicates for a given product isins (short, long)
        /// </summary>
        [HttpPost]
        [Route("duplicated-isins")]
        public async Task<List<string>> GetDuplicatedIsins([FromBody] string[] isins)
        {
            var result = await _assetsRepository.GetDuplicatedIsinsAsync(isins);

            return result.ToList();
        }

        /// <summary>
        /// Get the list of assets with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponse<AssetContract>> ListByPages(
            [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);
            
            var data = await _assetsRepository.GetByPagesAsync(skip, take);
            
            return new PaginatedResponse<AssetContract>(
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
        public async Task<Asset> GetLegacyAssetById(string assetId, bool startedOnly = true)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;

            var result = _legacyAssetsCache.GetById(assetId);

            if (result == null && !startedOnly)
            {
                var assets = await _legacyAssetsService.GetLegacyAssets(new[] { assetId }, false);
                result = assets.FirstOrDefault();
            }

            return result;
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