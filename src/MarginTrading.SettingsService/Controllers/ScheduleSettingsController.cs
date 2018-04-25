using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Client;
using MarginTrading.SettingsService.Client.Scheduling;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Schedule settings management
    /// </summary>
    [Route("api/scheduleSettings")]
    public class ScheduleSettingsController : Controller, IScheduleSettingsApi
    {
        private readonly IScheduleSettingsRepository _scheduleSettingsRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public ScheduleSettingsController(
            IScheduleSettingsRepository scheduleSettingsRepository,
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _scheduleSettingsRepository = scheduleSettingsRepository;
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of schedule settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<ScheduleSettingsContract>> List()
        {
            var data = await _scheduleSettingsRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<ScheduleSettings, ScheduleSettingsContract>(x)).ToList();
        }

        /// <summary>
        /// Create new schedule setting
        /// </summary>
        /// <param name="scheduleSetting"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ScheduleSettingsContract> Insert([FromBody] ScheduleSettingsContract scheduleSetting)
        {
            if (string.IsNullOrWhiteSpace(scheduleSetting?.Id))
            {
                throw new ArgumentNullException(nameof(scheduleSetting.Id), "scheduleSetting Id must be set");
            }

            await _scheduleSettingsRepository.InsertAsync(
                _convertService.Convert<ScheduleSettingsContract, ScheduleSettings>(scheduleSetting));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.ScheduleSettings);

            return scheduleSetting;
        }

        /// <summary>
        /// Get the schedule setting
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{settingId}")]
        public async Task<ScheduleSettingsContract> Get(string settingId)
        {
            var obj = await _scheduleSettingsRepository.GetAsync(settingId);
            
            return _convertService.Convert<ScheduleSettings, ScheduleSettingsContract>(obj);
        }

        /// <summary>
        /// Update the schedule setting
        /// </summary>
        /// <param name="settingId"></param>
        /// <param name="scheduleSetting"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{settingId}")]
        public async Task<ScheduleSettingsContract> Update(string settingId, 
            [FromBody] ScheduleSettingsContract scheduleSetting)
        {
            if (string.IsNullOrWhiteSpace(scheduleSetting?.Id))
            {
                throw new ArgumentNullException(nameof(scheduleSetting.Id), "scheduleSetting Id must be set");
            }

            await _scheduleSettingsRepository.ReplaceAsync(
                _convertService.Convert<ScheduleSettingsContract, ScheduleSettings>(scheduleSetting));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.ScheduleSettings);
            
            return scheduleSetting;
        }

        /// <summary>
        /// Delete the schedule setting
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{settingId}")]
        public async Task Delete(string settingId)
        {
            await _scheduleSettingsRepository.DeleteAsync(settingId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.ScheduleSettings);
        }

        /// <summary>
        /// Get the list of compiled schedule settings
        /// </summary>
        /// <param name="assetPairIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("compiled")]
        public async Task<List<CompiledScheduleContract>> StateList([FromBody] string[] assetPairIds)
        {
            var allSettingsTask = _scheduleSettingsRepository.GetAsync();
            var assetPairsTask = _assetPairsRepository.GetAsync(x => assetPairIds.Contains(x.Id));
            var allSettings = await allSettingsTask;
            var assetPairs = await assetPairsTask;
            
            //extract the list of assetpairs with same settings based on regex, market or list
            var result = assetPairIds.Select(assetPairId =>
            {
                var settings = allSettings.Where(setting => setting.AssetPairs.Contains(assetPairId)
                                                            || assetPairIds.Any(x =>
                                                                Regex.IsMatch(x, setting.AssetPairRegex,
                                                                    RegexOptions.IgnoreCase))
                                                            || assetPairs.Any(x => setting.MarketId == x.MarketId))
                    .ToList();
                return new CompiledScheduleContract
                {
                    AssetPairId = assetPairId,
                    ScheduleSettings = settings.Select(x =>
                        _convertService.Convert<ScheduleSettings, CompiledScheduleSettingsContract>(x)).ToList()
                };
            }).ToList();
            return result;
        }
    }
}