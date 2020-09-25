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
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
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
        private readonly IScheduleSettingsRepository _scheduleSettingsRepository;
        private readonly IMarketDayOffService _marketDayOffService;
        private readonly IConvertService _convertService;
        
        public ScheduleSettingsController(
            IScheduleSettingsRepository scheduleSettingsRepository,
            IMarketRepository marketRepository,
            IAssetPairsRepository assetPairsRepository,
            IMarketDayOffService marketDayOffService,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _scheduleSettingsRepository = scheduleSettingsRepository;
            _marketDayOffService = marketDayOffService;
            _convertService = convertService;
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