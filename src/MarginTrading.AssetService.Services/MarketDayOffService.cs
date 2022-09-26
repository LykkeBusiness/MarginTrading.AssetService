// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using Microsoft.Extensions.Internal;

namespace MarginTrading.AssetService.Services
{
    public class MarketDayOffService : IMarketDayOffService
    {
        private readonly IScheduleSettingsService _scheduleSettingsService;
        private readonly ISystemClock _systemClock;
        private readonly PlatformSettings _platformSettings;
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly string _brokerId;

        public MarketDayOffService(
            IScheduleSettingsService scheduleSettingsService,
            ISystemClock systemClock,
            PlatformSettings platformSettings, 
            string brokerId, 
            IBrokerSettingsApi brokerSettingsApi)
        {
            _scheduleSettingsService = scheduleSettingsService;
            _systemClock = systemClock;
            _platformSettings = platformSettings;
            _brokerId = brokerId;
            _brokerSettingsApi = brokerSettingsApi;
        }

        public async Task<Dictionary<string, TradingDayInfo>> GetMarketsInfo(string[] marketIds, DateTime? dateTime)
        {
            var scheduleSettings = (await _scheduleSettingsService.GetFilteredAsync())
                .Where(x => !string.IsNullOrWhiteSpace(x.MarketId))
                .Cast<ScheduleSettings>()
                .GroupBy(x => x.MarketId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var currentDateTime = dateTime ?? _systemClock.UtcNow.UtcDateTime;

            var rawPlatformSchedule =
                scheduleSettings.TryGetValue(_platformSettings.PlatformMarketId, out var platformSettings)
                    ? platformSettings
                    : new List<ScheduleSettings>();

            var brokerSettings = await GetBrokerSettings();

            var result = marketIds.Except(scheduleSettings.Keys).ToDictionary(
                marketWithoutSchedule => marketWithoutSchedule,
                _ => GetTradingDayInfo(rawPlatformSchedule, brokerSettings, currentDateTime));

            foreach (var marketToCompile in marketIds.Except(result.Keys))
            {
                var schedule = scheduleSettings[marketToCompile].Concat(
                    rawPlatformSchedule.WithRank(int.MaxValue)).ToList();

                var tradingDayInfo = GetTradingDayInfo(schedule, brokerSettings, currentDateTime);

                result.Add(marketToCompile, tradingDayInfo);
            }

            return result;
        }

        public async Task<TradingDayInfo> GetPlatformInfo(DateTime? dateTime)
        {
            var rawPlatformSchedule = (await _scheduleSettingsService.GetFilteredAsync())
                .Where(x => x.MarketId == _platformSettings.PlatformMarketId)
                .Cast<ScheduleSettings>()
                .ToList();
            var brokerSettings = await GetBrokerSettings();
            var currentDateTime = dateTime ?? _systemClock.UtcNow.UtcDateTime;

            return GetTradingDayInfo(rawPlatformSchedule, brokerSettings, currentDateTime);
        }

        private static TradingDayInfo GetTradingDayInfo(IEnumerable<ScheduleSettings> scheduleSettings, 
            BrokerSettingsContract brokerSettings, 
            DateTime currentDateTime)
        {
            var compiledSchedule = CompileSchedule(scheduleSettings, currentDateTime);
            
            var currentInterval = GetCurrentInterval(compiledSchedule, currentDateTime);

            var isEnabled = currentInterval.Enabled();
            var lastTradingDay = GetPreviousTradingDay(compiledSchedule, currentInterval, currentDateTime);
            var nextTradingDay = GetNextTradingDay(compiledSchedule, currentInterval, currentDateTime, lastTradingDay);
            var isBusinessDay = !brokerSettings.Weekends.Contains(currentDateTime.DayOfWeek)
                                && !brokerSettings.Holidays.Select(x => x.Date).Contains(currentDateTime.Date);

            var result = new TradingDayInfo
            {
                IsTradingEnabled = isEnabled,
                LastTradingDay = lastTradingDay,
                NextTradingDayStart = nextTradingDay,
                IsBusinessDay = isBusinessDay
            };

            return result;
        }

        private static CompiledScheduleTimeInterval GetCurrentInterval(
            IEnumerable<CompiledScheduleTimeInterval> intervals, DateTime currentDateTime)
        {
            return intervals
                .Where(x => IsBetween(currentDateTime, x.Start, x.End))
                .MaxBy(x => x.Schedule.Rank);
        }

        private static DateTime GetPreviousTradingDay(List<CompiledScheduleTimeInterval> compiledSchedule,
            CompiledScheduleTimeInterval currentInterval,
            DateTime currentDateTime)
        {
            if (currentInterval.Enabled())
                return currentDateTime.Date;

            var timestampBeforeCurrentIntervalStart = currentInterval.Start.AddTicks(-1);

            // search for the interval just before the current interval started
            var previousInterval = compiledSchedule
                .Where(x => IsBetween(timestampBeforeCurrentIntervalStart, x.Start, x.End))
                .MaxBy(x => x.Schedule.Rank);

            // if trading was enabled, then at that moment was the last trading day
            if (previousInterval.Enabled())
                return timestampBeforeCurrentIntervalStart.Date;

            if (previousInterval == null)
                throw new InvalidOperationException("No previous interval found");

            // if no, there was one more disabled interval and we should go next
            return GetPreviousTradingDay(compiledSchedule, previousInterval, previousInterval.Start);
        }

        private static DateTime GetNextTradingDay(List<CompiledScheduleTimeInterval>
            compiledSchedule, CompiledScheduleTimeInterval currentInterval, DateTime currentDateTime, DateTime lastTradingDay)
        {
            // search for the interval right after the current interval finished
            var ordered = compiledSchedule
                .Where(x => 
                    x.End > (currentInterval?.End ?? currentDateTime) || 
                    currentInterval != null && 
                    x.Schedule.Rank > currentInterval.Schedule.Rank && 
                    x.End > currentInterval.End)
                .OrderBy(x => x.Start)
                .ThenByDescending(x => x.Schedule.Rank)
                .ToList();
            
            var nextInterval = ordered.FirstOrDefault();
            
            if (nextInterval == null)
            {
                if (!currentInterval.Enabled() && currentInterval.End.Date > lastTradingDay.Date)
                {
                    return currentInterval.End;
                }

                // means no any intervals (current or any in the future)
                return currentDateTime.Date.AddDays(1);
            }

            var stateIsChangedToEnabled = nextInterval.Schedule.IsTradeEnabled != currentInterval.Enabled() && nextInterval.Enabled();
            var intervalIsMissing = currentInterval != null && nextInterval.Start > currentInterval.End;

            if (stateIsChangedToEnabled || intervalIsMissing && currentInterval.End.Date >= lastTradingDay.Date && !nextInterval.Enabled())
            {
                // ReSharper disable once PossibleNullReferenceException
                // if status was changed and next is enabled, that means current interval is disable == it not null
                return currentInterval.End;
            }

            // if we have long enabled interval with overnight, next day will start at 00:00:00
            if (currentInterval.Enabled() && currentDateTime.Date.AddDays(1) < nextInterval.Start)
            {
                return currentDateTime.Date.AddDays(1);
            }

            return GetNextTradingDay(compiledSchedule, nextInterval, nextInterval.End.AddTicks(1), lastTradingDay);
        }

        private static bool IsBetween(DateTime currentDateTime, DateTime start, DateTime end)
        {
            return start <= currentDateTime && currentDateTime < end;
        }

        private static List<CompiledScheduleTimeInterval> CompileSchedule(
            IEnumerable<ScheduleSettings> scheduleSettings, DateTime currentDateTime)
        {
            var scheduleSettingsByType = scheduleSettings
                .GroupBy(x => x.Start.GetConstraintType())
                .ToDictionary(x => x.Key, value => value);

            //handle weekly
            var weekly = scheduleSettingsByType.TryGetValue(ScheduleConstraintType.Weekly, out var weeklySchedule)
                ? weeklySchedule.SelectMany(sch =>
                {
                    // ReSharper disable PossibleInvalidOperationException - validated previously
                    var currentStart = CurrentWeekday(currentDateTime, sch.Start.DayOfWeek.Value)
                        .Add(sch.Start.Time.Subtract(sch.PendingOrdersCutOff ?? TimeSpan.Zero));
                    var currentEnd = CurrentWeekday(currentDateTime, sch.End.DayOfWeek.Value)
                        .Add(sch.End.Time.Add(sch.PendingOrdersCutOff ?? TimeSpan.Zero));

                    if (currentEnd < currentStart)
                    {
                        currentEnd = currentEnd.AddDays(7);
                    }

                    // By default, we use a range of (-7, 7) as of the current date to retrieve the nearest day-offs.
                    // In a case of long holidays (14+ days specifically) can cause irrelevant results.
                    return new[]
                    {
                        new CompiledScheduleTimeInterval(sch, currentStart.AddDays(-7), currentEnd.AddDays(-7)),
                        new CompiledScheduleTimeInterval(sch, currentStart, currentEnd),
                        new CompiledScheduleTimeInterval(sch, currentStart.AddDays(7), currentEnd.AddDays(7))
                    };
                })
                : new List<CompiledScheduleTimeInterval>();

            //handle single
            var single = scheduleSettingsByType.TryGetValue(ScheduleConstraintType.Single, out var singleSchedule)
                ? singleSchedule.Select(sch => new CompiledScheduleTimeInterval(sch,
                    sch.Start.Date.Value.Add(sch.Start.Time.Subtract(sch.PendingOrdersCutOff ?? TimeSpan.Zero)),
                    sch.End.Date.Value.Add(sch.End.Time.Add(sch.PendingOrdersCutOff ?? TimeSpan.Zero))))
                // ReSharper restore PossibleInvalidOperationException - validated previously
                : new List<CompiledScheduleTimeInterval>();

            var weeklyIntervals = weekly.ToArray();
            var singleIntervals = single.ToArray();
            var nearestGap = FindNearestGapAsOfNow(weeklyIntervals, singleIntervals, currentDateTime);

            //handle daily
            var daily = scheduleSettingsByType.TryGetValue(ScheduleConstraintType.Daily, out var dailySchedule)
                ? dailySchedule.SelectMany(sch =>
                {
                    var start = currentDateTime.Date.Add(sch.Start.Time);
                    var end = currentDateTime.Date.Add(sch.End.Time);
                    if (end < start)
                    {
                        end = end.AddDays(1);
                    }
                    
                    var result = new[]
                    {
                        new CompiledScheduleTimeInterval(sch, start.AddDays(-1), end.AddDays(-1)),
                        new CompiledScheduleTimeInterval(sch, start, end),
                        new CompiledScheduleTimeInterval(sch, start.AddDays(1), end.AddDays(1))
                    };
                    
                    var offsetToNearestGap = nearestGap != DateTime.MinValue 
                        ? (nearestGap.Date - start.Date).Days - 1
                        : 1;

                    if (offsetToNearestGap > 1)
                    {
                        return result.Concat(new[]
                        {
                            new CompiledScheduleTimeInterval(sch, start.AddDays(offsetToNearestGap),
                                end.AddDays(offsetToNearestGap))
                        });
                    } 

                    return result;
                })
                : new List<CompiledScheduleTimeInterval>();

            return weeklyIntervals.Concat(singleIntervals).Concat(daily).ToList();
        }

        /// <summary>
        /// Returns nearest gap for a given sets of weekly & single (combined together) day-offs
        /// </summary>
        private static DateTime FindNearestGapAsOfNow(IEnumerable<CompiledScheduleTimeInterval> weekly, 
            IEnumerable<CompiledScheduleTimeInterval> single, DateTime currentDate)
        {
            var intervals = weekly.Concat(single).ToList();
            if (!intervals.Any())
            {
                return DateTime.MinValue;
            }

            var currentInterval = GetCurrentInterval(intervals, currentDate);
            var lastTradingDay = GetPreviousTradingDay(intervals, currentInterval, currentDate);
            var nextTradingDay = GetNextTradingDay(intervals, currentInterval, currentDate, lastTradingDay);

            return nextTradingDay;
        }

        private static DateTime CurrentWeekday(DateTime start, DayOfWeek day)
        {
            return start.Date.AddDays((int) day - (int) start.DayOfWeek);
        }

        private async Task<BrokerSettingsContract> GetBrokerSettings()
        {
            var resp = await _brokerSettingsApi.GetByIdAsync(_brokerId);
            if (resp.ErrorCode != BrokerSettingsErrorCodesContract.None)
            {
                throw new InvalidOperationException($"Not able to retrieve broker settings for {_brokerId}, because of {resp.ErrorCode}");
            }

            return resp.BrokerSettings;
        }
    }
}