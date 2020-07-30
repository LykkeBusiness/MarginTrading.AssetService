// Copyright (c) 2019 Lykke Corp.
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
using MarginTrading.AssetService.Core.Domain;
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
        private readonly IEventSender _eventSender;

        public RateSettingsController(
            IRateSettingsService rateSettingsService,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _rateSettingsService = rateSettingsService;
            _convertService = convertService;
            _eventSender = eventSender;
        }

        [ProducesResponseType(typeof(IReadOnlyList<OrderExecutionRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-order-exec")]
        public async Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRatesAsync()
        {
            return (await _rateSettingsService.GetOrderExecutionRatesAsync())
                ?.Select(x => _convertService.Convert<OrderExecutionRate, OrderExecutionRateContract>(x)).ToList()
                   ?? new List<OrderExecutionRateContract>();
        }

        [ProducesResponseType(typeof(OrderExecutionRateContract), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-order-exec/{assetPairId}")]
        public async Task<OrderExecutionRateContract> GetOrderExecutionRateAsync(string assetPairId)
        {
            var executionRate = (await _rateSettingsService.GetOrderExecutionRatesAsync(new[] {assetPairId})).SingleOrDefault();

            if (executionRate == null)
                return null;

            return _convertService.Convert<OrderExecutionRate, OrderExecutionRateContract>(executionRate);
        }

        [ProducesResponseType(typeof(IReadOnlyList<OrderExecutionRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpPost("get-order-exec/list")]
        public async Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRatesAsync([FromBody] string[] assetPairIds)
        {
            var executionRates = await _rateSettingsService.GetOrderExecutionRatesAsync(assetPairIds);

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
        public async Task ReplaceOrderExecutionRatesAsync([FromBody] OrderExecutionRateContract[] rates)
        {
            if (rates == null || !rates.Any() || rates.Any(x => 
                    string.IsNullOrWhiteSpace(x.AssetPairId)
                    || string.IsNullOrWhiteSpace(x.CommissionAsset)))
            {
                throw new ArgumentNullException(nameof(rates));
            }

            await _rateSettingsService.ReplaceOrderExecutionRatesAsync(rates
                .Select(x => _convertService.Convert<OrderExecutionRateContract, OrderExecutionRate>(x))
                .ToList());

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.OrderExecution);
        }

        
        
        [ProducesResponseType(typeof(IReadOnlyList<OvernightSwapRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-overnight-swap")]
        public async Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync()
        {
            return (await _rateSettingsService.GetOvernightSwapRatesAsync())
                   ?.Select(x => _convertService.Convert<OvernightSwapRate, OvernightSwapRateContract>(x)).ToList()
                   ?? new List<OvernightSwapRateContract>();
        }

        [ProducesResponseType(typeof(OvernightSwapRateContract), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-overnight-swap/{assetPairId}")]
        public async Task<OvernightSwapRateContract> GetOvernightSwapRatesAsync(string assetPairId)
        {
            var swapRate = (await _rateSettingsService.GetOvernightSwapRatesAsync(new[] {assetPairId})).SingleOrDefault();

            if (swapRate == null)
                return null;

            return _convertService.Convert<OvernightSwapRate, OvernightSwapRateContract>(swapRate);
        }

        [ProducesResponseType(typeof(IReadOnlyList<OvernightSwapRateContract>), 200)]
        [ProducesResponseType(400)]
        [HttpPost("get-overnight-swap/list")]
        public async Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync(string[] assetPairIds)
        {
            var swapRates = await _rateSettingsService.GetOvernightSwapRatesAsync(assetPairIds);

            return swapRates
                .Select(_convertService.Convert<OvernightSwapRate, OvernightSwapRateContract>)
                .ToList();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("replace-overnight-swap")]
        public async Task ReplaceOvernightSwapRatesAsync([FromBody] OvernightSwapRateContract[] rates)
        {
            if (rates == null || !rates.Any() || rates.Any(x => 
                    string.IsNullOrWhiteSpace(x.AssetPairId)))
            {
                throw new ArgumentNullException(nameof(rates));
            }

            await _rateSettingsService.ReplaceOvernightSwapRatesAsync(rates
                .Select(x => _convertService.Convert<OvernightSwapRateContract, OvernightSwapRate>(x))
                .ToList());

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.OvernightSwap);
        }

        
        
        [ProducesResponseType(typeof(OnBehalfRateContract), 200)]
        [ProducesResponseType(400)]
        [HttpGet("get-on-behalf")]
        public async Task<OnBehalfRateContract> GetOnBehalfRateAsync()
        {
            var item = await _rateSettingsService.GetOnBehalfRateAsync();
            return item == null ? null : _convertService.Convert<OnBehalfRate, OnBehalfRateContract>(item);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [HttpPost("replace-on-behalf")]
        public async Task ReplaceOnBehalfRateAsync([FromBody] OnBehalfRateContract rate)
        {
            if (string.IsNullOrWhiteSpace(rate.CommissionAsset))
            {
                throw new ArgumentNullException(nameof(rate.CommissionAsset));
            }

            await _rateSettingsService.ReplaceOnBehalfRateAsync(
                _convertService.Convert<OnBehalfRateContract, OnBehalfRate>(rate));
            
            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.OnBehalf);
        }
    }
}