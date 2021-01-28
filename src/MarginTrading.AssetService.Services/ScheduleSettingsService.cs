using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Services.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class ScheduleSettingsService : IScheduleSettingsService
    {
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly PlatformSettings _platformSettings;
        private readonly string _brokerId;
        private readonly ILog _log;

        public ScheduleSettingsService(
            IBrokerSettingsApi brokerSettingsApi,
            IMarketSettingsRepository marketSettingsRepository,
            PlatformSettings platformSettings,
            string brokerId,
            ILog log)
        {
            _brokerSettingsApi = brokerSettingsApi;
            _marketSettingsRepository = marketSettingsRepository;
            _platformSettings = platformSettings;
            _brokerId = brokerId;
            _log = log;
        }

        public async Task<IReadOnlyList<IScheduleSettings>> GetFilteredAsync(string marketId = null)
        {
            var brokerSchedule = await GetBrokerScheduleAsync();
            
            if (!string.IsNullOrEmpty(marketId))
            {
                //We need only platform settings
                if (marketId == _platformSettings.PlatformMarketId)
                {
                    return MapPlatformSchedule(brokerSchedule);
                }

                var marketSettingsById = await _marketSettingsRepository.GetByIdAsync(marketId);
                if (marketSettingsById == null)
                    return new List<IScheduleSettings>();

                return MapMarketSchedule(marketSettingsById, brokerSchedule.Weekends);
            }
            
            // add all markets and platform schedules
            var allMarketSettings = await _marketSettingsRepository.GetAllMarketSettingsAsync();
            var allMarketSchedules = allMarketSettings
                .SelectMany(x => MapMarketSchedule(x, brokerSchedule.Weekends))
                .ToList();
            var platformSchedules = MapPlatformSchedule(brokerSchedule);
            allMarketSchedules.AddRange(platformSchedules);

            return allMarketSchedules;
        }

        private async Task<BrokerSettingsScheduleContract> GetBrokerScheduleAsync()
        {
            // Note: broker schedule is already UTC adjusted
            var result = await _brokerSettingsApi.GetScheduleInfoByIdAsync(_brokerId);

            if (result.ErrorCode != BrokerSettingsErrorCodesContract.None)
            {
                _log?.WriteErrorAsync(nameof(ScheduleSettingsService), nameof(GetBrokerScheduleAsync),
                    $"Schedule is missing for brokerId assigned to AssetService:{_brokerId}", null);
                throw new BrokerSettingsDoNotExistException();
            }

            return result.BrokerSettingsSchedule;
        }

        private List<ScheduleSettings> MapMarketSchedule(MarketSettings marketSettings, List<DayOfWeek> platformWeekends)
        {
            var result = MarketScheduleExtensions.MapHolidays(marketSettings.Id,
                marketSettings.Name,
                marketSettings.Holidays,
                null);
            
            result.AddRange(MarketScheduleExtensions.MapWeekends(
                marketSettings.Id, 
                marketSettings.Name, 
                platformWeekends,
                null));
            
            var marketScheduleUtcRespectful = marketSettings.MarketSchedule.ShiftToUtc();
            result.AddRange(
                marketScheduleUtcRespectful.GetMarketHoursScheduleSettings(marketSettings.Id,
                    marketSettings.Name,
                    null));
            
            result.AddRange(
                marketScheduleUtcRespectful.HalfWorkingDays.GetScheduleSettings(marketSettings.Id, 
                    marketSettings.Name, 
                    null));

            return result;
        }

        private IReadOnlyList<ScheduleSettings> MapPlatformSchedule(BrokerSettingsScheduleContract brokerSchedule)
        {
            var assetPairRegex = ".*";
            var platformId = _platformSettings.PlatformMarketId;
            var marketName = platformId;
            
            var result = MarketScheduleExtensions.MapHolidays(platformId, 
                marketName, 
                brokerSchedule.Holidays, 
                assetPairRegex);

            result.AddRange(MarketScheduleExtensions.MapWeekends(platformId, 
                marketName,
                brokerSchedule.Weekends,
                assetPairRegex));
            
            result.Add(
                MarketScheduleExtensions.GetSingleSessionScheduleSettings(platformId, 
                    marketName, 
                    assetPairRegex, 
                    brokerSchedule.Open, 
                    brokerSchedule.Close));
            
            result.AddRange(
                brokerSchedule.PlatformSchedule.HalfWorkingDays.GetScheduleSettings(platformId,
                    marketName,
                    assetPairRegex));

            return result;
        }
    }
}