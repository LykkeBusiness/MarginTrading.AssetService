// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
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
        private readonly IClientProfilesService _clientProfilesService;
        private readonly IConvertService _convertService;
        
        public TradingInstrumentsController(
            ITradingInstrumentsService tradingInstrumentsService,
            IConvertService convertService, 
            IClientProfilesService clientProfilesService)
        {
            _tradingInstrumentsService = tradingInstrumentsService;
            _convertService = convertService;
            _clientProfilesService = clientProfilesService;
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
        /// Returns trading instruments that are not available for a given client profile
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CheckProductsUnavailableForClientProfileResponse), (int) HttpStatusCode.OK)]
        [Route("unavailable")]
        public async Task<CheckProductsUnavailableForClientProfileResponse> CheckProductsUnavailableForTradingCondition(
            [FromBody] CheckProductsUnavailableForClientProfileRequest request)
        {
            var response = new CheckProductsUnavailableForClientProfileResponse();
            
            var clientProfile = await _clientProfilesService.GetByIdAsync(request.TradingConditionId);
            if (clientProfile == null)
            {
                response.ErrorCode = ClientProfilesErrorCodesContract.ClientProfileDoesNotExist;
                return response;
            }
            
            var unavailableProductIds = 
                await _tradingInstrumentsService.GetUnavailableProductsAsync(request.ProductIds, request.TradingConditionId);
            response.UnavailableProductIds = unavailableProductIds;

            return response;
        }
    }
}