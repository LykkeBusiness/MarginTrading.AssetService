using System;
using System.Collections.Generic;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class MarketScheduleExtensions
    {
        public static IReadOnlyList<ScheduleSettings> GetOpenCloseHoursScheduleSettings(
            this MarketSchedule marketSchedule,
            string marketId,
            string marketName,
            string assetRegex)
        {
            var result = new List<ScheduleSettings>();

            var daySessionsCount = marketSchedule.Open.Length;

            for (int i = 0; i < daySessionsCount; i++)
            {
                var ss = GetIntraDaySessionScheduleSettings(marketId, 
                    marketName, 
                    assetRegex, 
                    marketSchedule.Open[i],
                    marketSchedule.Close[i]);
                result.Add(ss);
            }

            return result;
        }

        public static ScheduleSettings GetIntraDaySessionScheduleSettings(string marketId,
            string marketName,
            string assetRegex,
            TimeSpan open,
            TimeSpan close)
        {
            return ScheduleSettings.Create(
                $"{marketId}_working_hours_open{open}_close{close}",
                marketId,
                marketName,
                new ScheduleConstraint {Time = close},
                new ScheduleConstraint {Time = open},
                assetRegex);
        }

        public static List<ScheduleSettings> MapHolidays(string marketId,
            string marketName,
            IEnumerable<DateTime> holidays,
            string assetPairRegex)
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
        
        public static ScheduleSettings MapWeekends(string marketId, string marketName, string assetPairRegex)
        {
            var start = new ScheduleConstraint
            {
                DayOfWeek = DayOfWeek.Saturday
            };
            var end = new ScheduleConstraint
            {
                DayOfWeek = DayOfWeek.Monday
            };
            var id = $"{marketId}_weekend";
            var settings = ScheduleSettings.Create(id, marketId, marketName, start, end, assetPairRegex);

            return settings;
        }
    }
}