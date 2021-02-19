// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Market;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Markets management
    /// </summary>
    [Authorize]
    [Route("api/markets")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class MarketsController : Controller, IMarketsApi
    {
        private readonly IMarketsService _marketsService;
        private readonly IConvertService _convertService;
        
        public MarketsController(
            IMarketsService marketsService,
            IConvertService convertService)
        {
            _marketsService = marketsService;
            _convertService = convertService;
        }
        
        /// <summary>
        /// Get the list of Markets
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<MarketContract>> List()
        {
            var data = await _marketsService.GetAllAsync();
            
            return data.Select(x => _convertService.Convert<IMarket, MarketContract>(x)).ToList();
        }

        /// <summary>
        /// Get the market
        /// </summary>
        [HttpGet]
        [Route("{marketId}")]
        public async Task<MarketContract> Get(string marketId)
        {
            var obj = await _marketsService.GetByIdAsync(marketId);
            
            return _convertService.Convert<IMarket, MarketContract>(obj);
        }
    }
}