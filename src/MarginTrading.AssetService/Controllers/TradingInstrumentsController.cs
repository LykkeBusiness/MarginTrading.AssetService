// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.TradingConditions;
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
    /// Trading instruments management
    /// </summary>
    [Authorize]
    [Route("api/tradingInstruments")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class TradingInstrumentsController : Controller, ITradingInstrumentsApi
    {
        private readonly ITradingInstrumentsRepository _tradingInstrumentsRepository;
        private readonly IConvertService _convertService;
        
        public TradingInstrumentsController(
            ITradingInstrumentsRepository tradingInstrumentsRepository,
            IConvertService convertService)
        {
            _tradingInstrumentsRepository = tradingInstrumentsRepository;
            _convertService = convertService;
        }
        
        /// <summary>
        /// Get the list of trading instruments
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<TradingInstrumentContract>> List([FromQuery] string tradingConditionId)
        {
            var data = string.IsNullOrWhiteSpace(tradingConditionId)
                ? await _tradingInstrumentsRepository.GetAsync()
                : await _tradingInstrumentsRepository.GetByTradingConditionAsync(tradingConditionId);
            
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
            
            var data = await _tradingInstrumentsRepository.GetByPagesAsync(tradingConditionId, skip, take);
            
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
            var obj = await _tradingInstrumentsRepository.GetAsync(assetPairId, tradingConditionId);

            return _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(obj);
        }
    }
}