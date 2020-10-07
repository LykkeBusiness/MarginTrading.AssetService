// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Scheduling;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Schedule settings management
    /// </summary>
    [Authorize]
    [Route("api/scheduleSettings")]
    public class ScheduleSettingsController : Controller, IScheduleSettingsApi
    {
        private readonly IScheduleSettingsService _scheduleSettingsService;
        private readonly IMarketSettingsService _marketSettingsService;
        private readonly IAssetPairService _assetPairsService;
        private readonly IMarketDayOffService _marketDayOffService;
        private readonly IConvertService _convertService;
        private readonly PlatformSettings _platformSettings;

        public ScheduleSettingsController(
            IScheduleSettingsService scheduleSettingsService,
            IMarketSettingsService marketSettingsService,
            IAssetPairService assetPairsService,
            IMarketDayOffService marketDayOffService,
            IConvertService convertService,
            PlatformSettings platformSettings)
        {
            _scheduleSettingsService = scheduleSettingsService;
            _marketSettingsService = marketSettingsService;
            _assetPairsService = assetPairsService;
            _marketDayOffService = marketDayOffService;
            _convertService = convertService;
            _platformSettings = platformSettings;
        }
        
        /// <summary>
        /// Get the list of schedule settings
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<ScheduleSettingsContract>> List([FromQuery] string marketId = null)
        {
            var data = await _scheduleSettingsService.GetFilteredAsync(marketId);
            return data
                .Select(x => _convertService.Convert<IScheduleSettings, ScheduleSettings>(x))
                .Select(x => _convertService.Convert<ScheduleSettings, ScheduleSettingsContract>(x))
                .ToList();
        }

        /// <summary>
        /// Get the list of compiled schedule settings based on array of asset pairs
        /// </summary>
        /// <param name="assetPairIds">Null by default</param>
        [HttpPost]
        [Route("compiled")]
        public async Task<List<CompiledScheduleContract>> StateList([FromBody] string[] assetPairIds)
        {
            var allSettings = await _scheduleSettingsService.GetFilteredAsync();
            var assetPairs = await _assetPairsService.GetByIdsAsync(assetPairIds);

            //extract the list of assetPairs with same settings based on regex, market or list
            var result = assetPairs.Select(assetPair => new CompiledScheduleContract
            {
                AssetPairId = assetPair.Id,
                ScheduleSettings = allSettings
                    .Where(setting => setting.AssetPairs.Contains(assetPair.Id)
                                      || (!string.IsNullOrWhiteSpace(setting.AssetPairRegex)
                                          && Regex.IsMatch(assetPair.Id,
                                              setting.AssetPairRegex,
                                              RegexOptions.IgnoreCase))
                                      || setting.MarketId == assetPair.MarketId)
                    .Select(x =>
                        _convertService.Convert<IScheduleSettings, CompiledScheduleSettingsContract>(x)).ToList()
            }).ToList();
            return result;
        }

        /// <summary>
        /// Get current trading status of markets. Platform schedule (with PlatformScheduleMarketId) overrides all others.
        /// </summary>
        /// <param name="marketIds">Optional. List of market Id's.</param>
        [HttpPost]
        [Route("markets-status")]
        [Obsolete]
        public async Task<Dictionary<string, bool>> MarketsStatus([FromBody] string[] marketIds = null)
        {
            var info = await GetMarketsInfo(marketIds);

            return info.ToDictionary(k => k.Key, v => v.Value.IsTradingEnabled);
        }

        /// <summary>
        /// Get current trading day info for markets. Platform schedule (with PlatformScheduleMarketId) overrides all others.
        /// </summary>
        /// <param name="marketIds">Optional. List of market Id's.</param>
        /// <param name="date">Timestamp of check (allows to check as at any moment of time)</param>
        [HttpPost]
        [Route("markets-info")]
        public async Task<Dictionary<string, TradingDayInfoContract>> GetMarketsInfo([FromBody] string[] marketIds = null, 
            [FromQuery] DateTime? date = null)
        {
            var allMarkets = (await _marketSettingsService.GetAllMarketSettingsAsync()).Select(x => x.Id).ToHashSet();
            allMarkets.Add(_platformSettings.PlatformMarketId);
            if (marketIds == null || !marketIds.Any())
            {
                marketIds = allMarkets.ToArray();
            }
            else
            {
                foreach (var marketId in marketIds)
                {
                    if (allMarkets.Contains(marketId))
                    {
                        continue;
                    }
                        
                    throw new ArgumentException($"Market {marketId} does not exist", nameof(marketIds));
                }
            }
            
            var info = await _marketDayOffService.GetMarketsInfo(marketIds, date);

            return info.ToDictionary(k => k.Key, v => new TradingDayInfoContract()
            {
                IsTradingEnabled = v.Value.IsTradingEnabled,
                LastTradingDay = v.Value.LastTradingDay,
                NextTradingDayStart = v.Value.NextTradingDayStart
            });
        }

        /// <summary>
        /// Get current platform trading info: is trading enabled, and last trading day.
        /// If interval has only date component i.e. time is 00:00:00.000, then previous day is returned.
        /// </summary>
        /// <param name="date">Timestamp of check (allows to check as at any moment of time)</param>
        [HttpGet]
        [Route("platform-info")]
        public async Task<TradingDayInfoContract> GetPlatformInfo([FromQuery] DateTime? date = null)
        {
            var info = await _marketDayOffService.GetPlatformInfo(date);

            return new TradingDayInfoContract
            {
                LastTradingDay = info.LastTradingDay,
                IsTradingEnabled = info.IsTradingEnabled,
                NextTradingDayStart = info.NextTradingDayStart
            };
        }
    }
}