// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.TradingConditions;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Trading instruments management
    /// </summary>
    [Authorize]
    [Route("api/tradingInstruments")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class TradingInstrumentsController : Controller, ITradingInstrumentsApi
    {
        private readonly ITradingInstrumentsService _tradingInstrumentsService;
        private readonly IConvertService _convertService;
        
        public TradingInstrumentsController(
            ITradingInstrumentsService tradingInstrumentsService,
            IConvertService convertService)
        {
            _tradingInstrumentsService = tradingInstrumentsService;
            _convertService = convertService;
        }
        
        /// <summary>
        /// Get the list of trading instruments
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<TradingInstrumentContract>> List([FromQuery] string tradingConditionId)
        {
            var data = await _tradingInstrumentsService.GetAsync(tradingConditionId);
            return data.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of trading instruments with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<TradingInstrumentContract>> ListByPages(string tradingConditionId, 
            int? skip = null, int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _tradingInstrumentsService.GetByPagesAsync(tradingConditionId, skip, take);
            
            return new PaginatedResponseContract<TradingInstrumentContract>(
                contents: data.Contents.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Get trading instrument
        /// </summary>
        [HttpGet]
        [Route("{tradingConditionId}/{assetPairId}")]
        public async Task<TradingInstrumentContract> Get(string tradingConditionId, string assetPairId)
        {
            var obj = await _tradingInstrumentsService.GetAsync(assetPairId, tradingConditionId);

            return _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(obj);
        }

        /// <summary>
        /// Get trading instruments that are not available on a specified trading condition
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("unavailable")]
        public async Task<CheckProductsUnavailableForTradingConditionResponse> CheckProductsUnavailableForTradingCondition(
            [FromBody] CheckProductsUnavailableForTradingConditionRequest request)
        {
            var unavailableProductIds =
                await _tradingInstrumentsService.GetUnavailableProductsAsync(request.ProductIds,
                    request.TradingConditionId);

            return new CheckProductsUnavailableForTradingConditionResponse
            {
                UnavailableProductIds = unavailableProductIds,
            };
        }
    }
}