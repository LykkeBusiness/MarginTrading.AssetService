// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Internal;

namespace MarginTrading.SettingsService.Services
{
    public class MarketDayOffService : IMarketDayOffService
    {
        private readonly IScheduleSettingsRepository _scheduleSettingsRepository;
        private readonly ISystemClock _systemClock;
        private readonly PlatformSettings _platformSettings;

        public MarketDayOffService(
            IScheduleSettingsRepository scheduleSettingsRepository,
            ISystemClock systemClock,
            PlatformSettings platformSettings)
        {
            _scheduleSettingsRepository = scheduleSettingsRepository;
            _systemClock = systemClock;
            _platformSettings = platformSettings;
        }

        public async Task<Dictionary<string, TradingDayInfo>> GetMarketsInfo(string[] marketIds)
        {
            var scheduleSettings = (await _scheduleSettingsRepository.GetFilteredAsync())
                .Where(x => !string.IsNullOrWhiteSpace(x.MarketId))
                .Cast<ScheduleSettings>()
                .GroupBy(x => x.MarketId)
                .ToDictionary(x => x.Key, x => x.ToList());
            var currentDateTime = _systemClock.UtcNow.UtcDateTime;

            var rawPlatformSchedule =
                scheduleSettings.TryGetValue(_platformSettings.PlatformMarketId, out var platformSettings)
                    ? platformSettings
                    : new List<ScheduleSettings>();

            var result = marketIds.Except(scheduleSettings.Keys).ToDictionary(
                marketWithoutSchedule => marketWithoutSchedule,
                _ => GetTradingDayInfo(rawPlatformSchedule, currentDateTime));

            foreach (var marketToCompile in marketIds.Except(result.Keys))
            {
                var schedule = scheduleSettings[marketToCompile].Concat(
                    rawPlatformSchedule.WithRank(int.MaxValue)).ToList();

                var tradingDayInfo = GetTradingDayInfo(schedule, currentDateTime);

                result.Add(marketToCompile, tradingDayInfo);
            }

            return result;
        }

        public async Task<TradingDayInfo> GetPlatformInfo()
        {
            var rawPlatformSchedule = (await _scheduleSettingsRepository.GetFilteredAsync())
                .Where(x => x.MarketId == _platformSettings.PlatformMarketId)
                .Cast<ScheduleSettings>()
                .ToList();
            var currentDateTime = _systemClock.UtcNow.UtcDateTime;

            return GetTradingDayInfo(rawPlatformSchedule, currentDateTime);
        }

        private static TradingDayInfo GetTradingDayInfo(
            IEnumerable<ScheduleSettings> scheduleSettings, DateTime currentDateTime)
        {
            var compiledSchedule = CompileSchedule(scheduleSettings, currentDateTime);

            var currentInterval = compiledSchedule
                .Where(x => IsBetween(currentDateTime, x.Start, x.End))
                .OrderByDescending(x => x.Schedule.Rank)
                .FirstOrDefault();

            var isEnabled = currentInterval?.Schedule.IsTradeEnabled ?? true;
            var lastTradingDay = GetPreviousTradingDay(compiledSchedule, currentInterval, currentDateTime);
            var nextTradingDay = GetNextTradingDay(compiledSchedule, currentInterval, currentDateTime);    

            var result = new TradingDayInfo
            {
                IsTradingEnabled = isEnabled,
                LastTradingDay = lastTradingDay,
                NextTradingDayStart = nextTradingDay
            };

            return result;
        }

        private static DateTime GetPreviousTradingDay(List<CompiledScheduleTimeInterval>
            compiledSchedule, CompiledScheduleTimeInterval currentInterval, DateTime currentDateTime)
        {
            if (currentInterval?.Schedule.IsTradeEnabled ?? true)
                return currentDateTime.Date;
            
            var timestampBeforeCurrentIntervalStart = currentInterval.Start.AddTicks(-1);

            // search for the interval just before the current interval started
            var previousInterval = compiledSchedule
                .Where(x => IsBetween(timestampBeforeCurrentIntervalStart, x.Start, x.End))
                .OrderByDescending(x => x.Schedule.Rank)
                .FirstOrDefault();

            // if trading was enabled, then at that moment was the last trading day
            if (previousInterval?.Schedule.IsTradeEnabled ?? true)
                return timestampBeforeCurrentIntervalStart.Date;

            // if no, there was one more disabled interval and we should go next
            return GetPreviousTradingDay(compiledSchedule, previousInterval, previousInterval.Start);
        }

        private static DateTime GetNextTradingDay(List<CompiledScheduleTimeInterval>
            compiledSchedule, CompiledScheduleTimeInterval currentInterval, DateTime currentDateTime)
        {
            // search for the interval right after the current interval finished
            var nextInterval = compiledSchedule
                .Where(x => x.End > (currentInterval?.End ?? currentDateTime)
                            || currentInterval != null && x.Schedule.Rank > currentInterval.Schedule.Rank)
                .OrderByDescending(x => x.Schedule.Rank)
                .ThenBy(x => x.Start)
                .FirstOrDefault();
            
            if (nextInterval == null)
            {
                if (currentInterval?.Schedule.IsTradeEnabled == false)
                {
                    return currentInterval.End;
                }
                else // means no any intervals (current or any in the future)
                {
                    return currentDateTime.AddDays(1); 
                }
            }

            var stateIsChangedToEnabled =
                nextInterval.Schedule.IsTradeEnabled != currentInterval?.Schedule.IsTradeEnabled
                && (nextInterval.Schedule.IsTradeEnabled ?? true);
            var intervalIsMissing = currentInterval != null && nextInterval.Start > currentInterval.End;

            if (stateIsChangedToEnabled || intervalIsMissing)
            {
                // ReSharper disable once PossibleNullReferenceException
                // if status was changed and next is enabled, that means current interval is disable == it not null
                return currentInterval.End;
            }

            return GetNextTradingDay(compiledSchedule, nextInterval, nextInterval.End.AddTicks(1));
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

                    return new[]
                    {
                        new CompiledScheduleTimeInterval(sch, start.AddDays(-1), end.AddDays(-1)),
                        new CompiledScheduleTimeInterval(sch, start, end),
                        new CompiledScheduleTimeInterval(sch, start.AddDays(1), end.AddDays(1))
                    };
                })
                : new List<CompiledScheduleTimeInterval>();

            return weekly.Concat(single).Concat(daily).ToList();
        }

        private static DateTime CurrentWeekday(DateTime start, DayOfWeek day)
        {
            return start.Date.AddDays((int) day - (int) start.DayOfWeek);
        }
    }
}