﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Rates;
using MarginTrading.AssetService.Core.Domain.Rates;
using Microsoft.AspNetCore.Authorization;

namespace MarginTrading.AssetService.Controllers
{
    [Authorize]
    [Route("api/rates")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class RateSettingsController : Controller, IRateSettingsApi
    {
        private readonly IRateSettingsService _rateSettingsService;
        private readonly IConvertService _convertService;

        public RateSettingsController(
            IRateSettingsService rateSettingsService,
            IConvertService convertService)
        {
            _rateSettingsService = rateSettingsService;
            _convertService = convertService;
        }

        [ProducesResponseType(typeof(IReadOnlyList<OrderExecutionRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-order-exec")]
        public async Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRates()
        {
            return (await _rateSettingsService.GetOrderExecutionRates())
                ?.Select(x => _convertService.Convert<OrderExecutionRate, OrderExecutionRateContract>(x)).ToList()
                   ?? new List<OrderExecutionRateContract>();
        }

        [ProducesResponseType(typeof(OrderExecutionRateContract), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-order-exec/{assetPairId}")]
        public async Task<OrderExecutionRateContract> GetOrderExecutionRate(string assetPairId)
        {
            var executionRate = (await _rateSettingsService.GetOrderExecutionRates(new[] {assetPairId})).SingleOrDefault();

            if (executionRate == null)
                return null;

            return _convertService.Convert<OrderExecutionRate, OrderExecutionRateContract>(executionRate);
        }

        [ProducesResponseType(typeof(IReadOnlyList<OrderExecutionRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-order-exec/list")]
        public async Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRates([FromQuery] string[] assetPairIds)
        {
            var executionRates = await _rateSettingsService.GetOrderExecutionRates(assetPairIds);

            return executionRates
                .Select(_convertService.Convert<OrderExecutionRate, OrderExecutionRateContract>)
                .ToList();
        }

        /// <summary>
        /// Replace order execution rates
        /// </summary>
        /// <param name="rates"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("replace-order-exec")]
        public async Task ReplaceOrderExecutionRates([FromBody] OrderExecutionRateContract[] rates)
        {
            if (rates == null || !rates.Any() || rates.Any(x => 
                    string.IsNullOrWhiteSpace(x.AssetPairId)
                    || string.IsNullOrWhiteSpace(x.CommissionAsset)))
            {
                throw new ArgumentNullException(nameof(rates));
            }

            await _rateSettingsService.ReplaceOrderExecutionRates(rates
                .Select(x => _convertService.Convert<OrderExecutionRateContract, OrderExecutionRate>(x))
                .ToList());
        }

        
        
        [ProducesResponseType(typeof(IReadOnlyList<OvernightSwapRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-overnight-swap")]
        public async Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRates()
        {
            return (await _rateSettingsService.GetOvernightSwapRates())
                   ?.Select(x => _convertService.Convert<OvernightSwapRate, OvernightSwapRateContract>(x)).ToList()
                   ?? new List<OvernightSwapRateContract>();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("replace-overnight-swap")]
        public async Task ReplaceOvernightSwapRates([FromBody] OvernightSwapRateContract[] rates)
        {
            if (rates == null || !rates.Any() || rates.Any(x => 
                    string.IsNullOrWhiteSpace(x.AssetPairId)))
            {
                throw new ArgumentNullException(nameof(rates));
            }

            await _rateSettingsService.ReplaceOvernightSwapRates(rates
                .Select(x => _convertService.Convert<OvernightSwapRateContract, OvernightSwapRate>(x))
                .ToList());
        }

        
        
        [ProducesResponseType(typeof(OnBehalfRateContract), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-on-behalf")]
        public async Task<OnBehalfRateContract> GetOnBehalfRate()
        {
            var item = await _rateSettingsService.GetOnBehalfRate();
            return item == null ? null : _convertService.Convert<OnBehalfRate, OnBehalfRateContract>(item);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("replace-on-behalf")]
        public async Task ReplaceOnBehalfRate([FromBody] OnBehalfRateContract rate)
        {
            if (string.IsNullOrWhiteSpace(rate.CommissionAsset))
            {
                throw new ArgumentNullException(nameof(rate.CommissionAsset));
            }

            await _rateSettingsService.ReplaceOnBehalfRate(
                _convertService.Convert<OnBehalfRateContract, OnBehalfRate>(rate));
        }
    }
}