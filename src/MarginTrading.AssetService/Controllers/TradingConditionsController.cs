// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
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
    /// Trading conditions management
    /// </summary>
    [Authorize]
    [Route("api/tradingConditions")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class TradingConditionsController : Controller, ITradingConditionsApi
    {
        private readonly ITradingConditionsService _tradingConditionsService;
        private readonly IConvertService _convertService;
        
        public TradingConditionsController(
            ITradingConditionsService tradingConditionsService,
            IConvertService convertService)
        {
            _tradingConditionsService = tradingConditionsService;
            _convertService = convertService;
        }
        
        /// <summary>
        /// Get the list of trading conditions
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<TradingConditionContract>> List([FromQuery] bool? isDefault = null)
        {
            var data = isDefault.HasValue ? 
                await _tradingConditionsService.GetByDefaultFilterAsync(isDefault.Value):
                await _tradingConditionsService.GetAsync();
            
            return data
                .Select(x => _convertService.Convert<ITradingCondition, TradingConditionContract>(x)).ToList();
        }

        /// <summary>
        /// Get the trading condition
        /// </summary>
        [HttpGet]
        [Route("{tradingConditionId}")]
        public async Task<TradingConditionContract> Get(string tradingConditionId)
        {
            var obj = await _tradingConditionsService.GetAsync(tradingConditionId);
            
            return _convertService.Convert<ITradingCondition, TradingConditionContract>(obj);
        }
    }
}