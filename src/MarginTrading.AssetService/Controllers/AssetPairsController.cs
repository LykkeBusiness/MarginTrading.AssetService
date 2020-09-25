// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Asset pairs management
    /// </summary>
    [Authorize]
    [Route("api/assetPairs")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class AssetPairsController : Controller, IAssetPairsApi
    {
        private readonly IAssetPairService _assetPairsService;
        private readonly IConvertService _convertService;
        
        public AssetPairsController(
            IAssetPairService assetPairsService,
            IConvertService convertService)
        {
            _assetPairsService = assetPairsService;
            _convertService = convertService;
        }

        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode
        /// </summary>
        /// <param name="legalEntity"></param>
        /// <param name="matchingEngineMode"></param>
        /// <param name="filter">Search by Id and Name</param>
        [HttpGet]
        [Route("")]
        public async Task<List<AssetPairContract>> List([FromQuery] string legalEntity = null,
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null, string filter = null)
        {
            //Some filters are ignored because they are not relevant anymore
            var data = await _assetPairsService.GetWithFilterAsync(filter);
            
            return data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        /// <param name="legalEntity"></param>
        /// <param name="matchingEngineMode"></param>
        /// <param name="filter">Search by Id and Name</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<AssetPairContract>> ListByPages([FromQuery] string legalEntity = null, 
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null, [FromQuery] string filter = null,
            [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            //Some filters are ignored because they are not relevant anymore

            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _assetPairsService.GetPaginatedWithFilterAsync(filter, skip, take);
            
            return new PaginatedResponseContract<AssetPairContract>(
                contents: data.Contents.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Get asset pair by id
        /// </summary>
        [HttpGet]
        [Route("{assetPairId}")]
        public async Task<AssetPairContract> Get(string assetPairId)
        {
            var obj = await _assetPairsService.GetByIdAsync(assetPairId);
            return _convertService.Convert<IAssetPair, AssetPairContract>(obj);
        }
    }
}