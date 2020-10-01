// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Asset;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asset = Cronut.Dto.Assets.Asset;

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
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly IAssetsRepository _assetsRepository;
        private readonly IConvertService _convertService;
        
        public AssetsController(
            ILegacyAssetsService legacyAssetsService,
            IAssetsRepository assetsRepository,
            IConvertService convertService)
        {
            _legacyAssetsService = legacyAssetsService;
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
        /// Get the list of assets
        /// </summary>
        [HttpGet]
        [Route("legacy")]
        public async Task<List<Asset>> GetLegacyAssets()
        {
            var result = await _legacyAssetsService.GetLegacyAssets();

            return result;
        }
    }
}