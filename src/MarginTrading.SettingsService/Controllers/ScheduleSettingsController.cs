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
        private readonly IMarketRepository _marketRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public ScheduleSettingsController(
            IScheduleSettingsRepository scheduleSettingsRepository,
            IMarketRepository marketRepository,
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _scheduleSettingsRepository = scheduleSettingsRepository;
            _marketRepository = marketRepository;
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
            return data
                .Select(x => _convertService.Convert<IScheduleSettings, ScheduleSettings>(x))
                .Select(x => _convertService.Convert<ScheduleSettings, ScheduleSettingsContract>(x))
                .ToList();
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
            await ValidateScheduleSettings(scheduleSetting);

            if (!await _scheduleSettingsRepository.TryInsertAsync(
                _convertService.Convert<ScheduleSettingsContract, ScheduleSettings>(scheduleSetting)))
            {
                throw new ArgumentException($"Schedule setting with id {scheduleSetting.Id} already exists",
                    nameof(scheduleSetting.Id));
            }

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
            
            return _convertService.Convert<IScheduleSettings, ScheduleSettingsContract>(obj);
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
            await ValidateScheduleSettings(scheduleSetting);
            
            ValidateId(settingId, scheduleSetting);
            
            await _scheduleSettingsRepository.UpdateAsync(
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
        /// Get the list of compiled schedule settings based on array of asset pairs
        /// </summary>
        /// <param name="assetPairIds">Null by default</param>
        /// <returns></returns>
        [HttpPost]
        [Route("compiled")]
        public async Task<List<CompiledScheduleContract>> StateList([FromBody] string[] assetPairIds)
        {
            var allSettingsTask = _scheduleSettingsRepository.GetAsync();
            var assetPairsTask = _assetPairsRepository.GetAsync(assetPairIds);
            var allSettings = await allSettingsTask;
            var assetPairs = await assetPairsTask;
            
            //extract the list of assetpairs with same settings based on regex, market or list
            var result = assetPairs.Select(assetPair => new CompiledScheduleContract
            {
                AssetPairId = assetPair.Id,
                ScheduleSettings = allSettings
                    .Where(setting => setting.AssetPairs.Contains(assetPair.Id)
                                      || Regex.IsMatch(assetPair.Id,
                                          setting.AssetPairRegex,
                                          RegexOptions.IgnoreCase)
                                      || setting.MarketId == assetPair.MarketId)
                    .Select(x =>
                        _convertService.Convert<IScheduleSettings, CompiledScheduleSettingsContract>(x)).ToList()
            }).ToList();
            return result;
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
                throw new ArgumentNullException("scheduleSetting", "Model is incorrect");
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

            if (scheduleSetting.Start == null || scheduleSetting.End == null)
            {
                throw new InvalidOperationException($"Start and End must be set");
            }

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