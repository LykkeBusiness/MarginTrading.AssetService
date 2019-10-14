// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.Scheduling;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Middleware;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Schedule settings management
    /// </summary>
    [Authorize]
    [Route("api/scheduleSettings")]
    public class ScheduleSettingsController : Controller, IScheduleSettingsApi
    {
        private readonly IScheduleSettingsRepository _scheduleSettingsRepository;
        private readonly IMarketRepository _marketRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IMarketDayOffService _marketDayOffService;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public ScheduleSettingsController(
            IScheduleSettingsRepository scheduleSettingsRepository,
            IMarketRepository marketRepository,
            IAssetPairsRepository assetPairsRepository,
            IMarketDayOffService marketDayOffService,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _scheduleSettingsRepository = scheduleSettingsRepository;
            _marketRepository = marketRepository;
            _assetPairsRepository = assetPairsRepository;
            _marketDayOffService = marketDayOffService;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of schedule settings
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<ScheduleSettingsContract>> List([FromQuery] string marketId = null)
        {
            var data = await _scheduleSettingsRepository.GetFilteredAsync(marketId);
            return data
                .Select(x => _convertService.Convert<IScheduleSettings, ScheduleSettings>(x))
                .Select(x => _convertService.Convert<ScheduleSettings, ScheduleSettingsContract>(x))
                .ToList();
        }

        /// <summary>
        /// Create new schedule setting
        /// </summary>
        [HttpPost]
        [Route("")]
        [MiddlewareFilter(typeof(RequestLoggingPipeline))]
        public async Task<ScheduleSettingsContract> Insert([FromBody] ScheduleSettingsContract scheduleSetting)
        {
            await ValidateScheduleSettings(scheduleSetting);

            if (!await _scheduleSettingsRepository.TryInsertAsync(
                _convertService.Convert<ScheduleSettingsContract, ScheduleSettings>(scheduleSetting)))
            {
                throw new ArgumentException($"Schedule setting with id {scheduleSetting.Id} already exists",
                    nameof(scheduleSetting.Id));
            }

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.ScheduleSettings, scheduleSetting.Id);

            return scheduleSetting;
        }

        /// <summary>
        /// Get the schedule setting
        /// </summary>
        [HttpGet]
        [Route("{settingId}")]
        public async Task<ScheduleSettingsContract> Get(string settingId)
        {
            var obj = await _scheduleSettingsRepository.GetAsync(settingId);
            
            return _convertService.Convert<IScheduleSettings, ScheduleSettingsContract>(obj);
        }

        /// <summary>
        /// Update the schedule setting
        /// </summary>
        [HttpPut]
        [Route("{settingId}")]
        [MiddlewareFilter(typeof(RequestLoggingPipeline))]
        public async Task<ScheduleSettingsContract> Update(string settingId, 
            [FromBody] ScheduleSettingsContract scheduleSetting)
        {
            await ValidateScheduleSettings(scheduleSetting);
            
            ValidateId(settingId, scheduleSetting);
            
            await _scheduleSettingsRepository.UpdateAsync(
                _convertService.Convert<ScheduleSettingsContract, ScheduleSettings>(scheduleSetting));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.ScheduleSettings, settingId);
            
            return scheduleSetting;
        }

        /// <summary>
        /// Delete the schedule setting
        /// </summary>
        [HttpDelete]
        [Route("{settingId}")]
        [MiddlewareFilter(typeof(RequestLoggingPipeline))]
        public async Task Delete(string settingId)
        {
            await _scheduleSettingsRepository.DeleteAsync(settingId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.ScheduleSettings, settingId);
        }

        /// <summary>
        /// Get the list of compiled schedule settings based on array of asset pairs
        /// </summary>
        /// <param name="assetPairIds">Null by default</param>
        [HttpPost]
        [Route("compiled")]
        public async Task<List<CompiledScheduleContract>> StateList([FromBody] string[] assetPairIds)
        {
            var allSettingsTask = _scheduleSettingsRepository.GetFilteredAsync();
            var assetPairsTask = _assetPairsRepository.GetAsync(assetPairIds);
            var allSettings = await allSettingsTask;
            var assetPairs = await assetPairsTask;
            
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
            var allMarkets = (await _marketRepository.GetAsync()).Select(x => x.Id).ToHashSet();
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

        private void ValidateId(string id, ScheduleSettingsContract contract)
        {
            if (contract?.Id != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }

        private async Task ValidateScheduleSettings(ScheduleSettingsContract scheduleSetting)
        {
            if (scheduleSetting == null)
            {
                throw new ArgumentNullException(nameof(scheduleSetting), "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(scheduleSetting.Id))
            {
                throw new ArgumentNullException(nameof(scheduleSetting.Id), "scheduleSetting Id must be set");
            }

            if (!string.IsNullOrEmpty(scheduleSetting.MarketId)
                && await _marketRepository.GetAsync(scheduleSetting.MarketId) == null)
            {
                throw new InvalidOperationException($"Market {scheduleSetting.MarketId} does not exist");
            }

            ScheduleConstraintContract.Validate(scheduleSetting);

            if (scheduleSetting.Start.DayOfWeek != null && !Enum.IsDefined(typeof(DayOfWeek), scheduleSetting.Start.DayOfWeek))
            {
                throw new ArgumentNullException(nameof(scheduleSetting.Start.DayOfWeek), "AssetPair Start DayOfWeek is set to an incorrect value");
            }

            if (scheduleSetting.End.DayOfWeek != null && !Enum.IsDefined(typeof(DayOfWeek), scheduleSetting.End.DayOfWeek))
            {
                throw new ArgumentNullException(nameof(scheduleSetting.End.DayOfWeek), "AssetPair End DayOfWeek is set to an incorrect value");
            }

            foreach (var assetPair in scheduleSetting.AssetPairs)
            {
                if (await _assetPairsRepository.GetAsync(assetPair) == null)
                {
                    throw new InvalidOperationException($"Asset pair {assetPair} does not exist");
                }
            }
        }
    }
}