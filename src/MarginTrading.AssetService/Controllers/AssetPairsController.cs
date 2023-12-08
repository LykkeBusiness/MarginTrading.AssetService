// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Middleware;
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
        [HttpGet]
        [Route("")]
        public async Task<List<AssetPairContract>> List()
        {
            //Some filters are ignored because they are not relevant anymore
            var data = await _assetPairsService.GetAllIncludingFxParisWithFilterAsync();
            
            return data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        [HttpGet]
        [Obsolete("Use /api/assetPairs since result from this method its not paginated anyway.")]
        [Route("by-pages")]
        public async Task<PaginatedResponse<AssetPairContract>> ListByPages([FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            //Some filters are ignored because they are not relevant anymore

            var data = await _assetPairsService.GetAllIncludingFxParisWithFilterAsync();
            var count = data.Count;
            //Just proxy of get all without actual pagination, done this way to avoid breaking changes
            return new PaginatedResponse<AssetPairContract>(
                contents: data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList(),
                start: 0,
                size: count,
                totalSize: count
            );
        }
    }
}