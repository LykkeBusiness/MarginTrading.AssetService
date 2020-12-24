using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Snow.Common.Extensions;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using TimeZoneConverter;

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
            var brokerSettings = await GetBrokerSettingsAsync();

            if (!string.IsNullOrEmpty(marketId))
            {
                //We need only platform settings
                if (marketId == _platformSettings.PlatformMarketId)
                    return MapPlatformScheduleSettings(brokerSettings.Open, brokerSettings.Close, brokerSettings.Timezone,
                        brokerSettings.Holidays, brokerSettings.Weekends);

                var marketSettingsById = await _marketSettingsRepository.GetByIdAsync(marketId);
                if (marketSettingsById == null)
                    return new List<IScheduleSettings>();

                return MapMarketScheduleSettings(marketSettingsById.Id,  marketSettingsById.Name, marketSettingsById.Open,
                    marketSettingsById.Close, marketSettingsById.Timezone, marketSettingsById.Holidays, brokerSettings.Weekends);
            }

            var platformSettings = MapPlatformScheduleSettings(brokerSettings.Open, brokerSettings.Close, brokerSettings.Timezone,
                brokerSettings.Holidays, brokerSettings.Weekends);
            var allMarketSettings = await _marketSettingsRepository.GetAllMarketSettingsAsync();

            var result = allMarketSettings
                .SelectMany(x => MapMarketScheduleSettings(x.Id, x.Name, x.Open, x.Close, x.Timezone, x.Holidays, brokerSettings.Weekends)).ToList();
            result.AddRange(platformSettings);

            return result;
        }

        private async Task<BrokerSettingsContract> GetBrokerSettingsAsync()
        {
            var result = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (result.ErrorCode != BrokerSettingsErrorCodesContract.None)
            {
                _log?.WriteErrorAsync(nameof(ScheduleSettingsService), nameof(GetBrokerSettingsAsync),
                    $"Broker settings missing for brokerId assigned to AssetService:{_brokerId}", null);
                throw new BrokerSettingsDoNotExistException();
            }

            return result.BrokerSettings;
        }

        private List<ScheduleSettings> MapMarketScheduleSettings(string marketId, string marketName, TimeSpan open,
            TimeSpan close, string timezone, IEnumerable<DateTime> holidays, List<DayOfWeek> brokerSettingsWeekends)
        {
            var result = MapHolidays(marketId, marketName, holidays, null);
            result.AddRange(MapWeekendHolidays(marketId, marketName, null, brokerSettingsWeekends));
            result.Add(MapClosedHours(marketId, marketName, open, close, timezone, null));

            return result;
        }

        private List<ScheduleSettings> MapPlatformScheduleSettings(TimeSpan open, TimeSpan close, string timezone,
            IEnumerable<DateTime> holidays, List<DayOfWeek> brokerSettingsWeekends)
        {
            var assetPairRegex = ".*";
            var platformId = _platformSettings.PlatformMarketId;
            var marketName = platformId;
            var result = MapHolidays(platformId, marketName, holidays, assetPairRegex);
            result.AddRange(MapWeekendHolidays(platformId, marketName, assetPairRegex, brokerSettingsWeekends));
            result.Add(MapClosedHours(platformId, marketName, open, close, timezone, assetPairRegex));

            return result;
        }

        private List<ScheduleSettings> MapWeekendHolidays(string marketId, string marketName, string assetPairRegex,
            List<DayOfWeek> brokerSettingsWeekends)
        {
            var result = new List<ScheduleSettings>();
            
            foreach (var weekend in brokerSettingsWeekends)
            {
                var start = new ScheduleConstraint
                {
                    DayOfWeek = weekend
                };
                var end = new ScheduleConstraint
                {
                    DayOfWeek = weekend == DayOfWeek.Saturday ? DayOfWeek.Sunday : weekend + 1
                };
                var id = $"{marketId}_{weekend.ToString()}";
                var settings = ScheduleSettings.Create(id, marketId, marketName, start, end, assetPairRegex);

                result.Add(settings);
            }
            
            return result;
        }

        private List<ScheduleSettings> MapHolidays(string marketId, string marketName, IEnumerable<DateTime> holidays, string assetPairRegex)
        {
            var result = new List<ScheduleSettings>();

            foreach (var holiday in holidays)
            {
                var start = new ScheduleConstraint
                {
                    Date = holiday.Date
                };
                var end = new ScheduleConstraint
                {
                    Date = holiday.Date.AddDays(1)
                };
                var id = $"{marketId}_holiday_{holiday.Date}";
                result.Add(ScheduleSettings.Create(id, marketId, marketName, start, end, assetPairRegex));
            }

            return result;
        }

        private ScheduleSettings MapClosedHours(string marketId, string marketName, TimeSpan open, TimeSpan close, string timezone, string assetPairRegex)
        {
            var timezoneInfo = TZConvert.GetTimeZoneInfo(timezone);
            var openUtc = open.ShiftToUtc(timezoneInfo);
            var closeUtc = close.ShiftToUtc(timezoneInfo);
            //Maybe we can have more than a day after we apply timezone, so we need to remove day portion
            var openUtcWithoutDays = new TimeSpan(openUtc.Hours, openUtc.Minutes, openUtc.Seconds);
            var closeUtcWithoutDays = new TimeSpan(closeUtc.Hours, closeUtc.Minutes, closeUtc.Seconds);

            var start = new ScheduleConstraint
            {
                Time = closeUtcWithoutDays
            };
            var end = new ScheduleConstraint
            {
                Time = openUtcWithoutDays
            };
            var id = $"{marketId}_working_hours_open{openUtcWithoutDays}_close{closeUtcWithoutDays}";
            var settings = ScheduleSettings.Create(id, marketId, marketName, start, end, assetPairRegex);

            return settings;
        }
    }
}