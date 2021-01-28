using System;
using System.Collections.Generic;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class MarketScheduleExtensions
    {
        public static IReadOnlyList<ScheduleSettings> GetMarketHoursScheduleSettings(
            this MarketSchedule marketSchedule,
            string marketId,
            string marketName,
            string assetRegex)
        {
            var result = new List<ScheduleSettings>();

            var daySessionsCount = marketSchedule.Open.Length;
            
            for (int i = 0; i < daySessionsCount; i++)
            {
                var isLastTradingSession = i == daySessionsCount - 1;
                var sessionClose = marketSchedule.Close[i];
                var nextSessionOpen = isLastTradingSession 
                    ? marketSchedule.Open[0] 
                    : marketSchedule.Open[i + 1];

                var ss = ScheduleSettings.Create(
                    $"{marketId}_none_working_hours_open{sessionClose}_close{nextSessionOpen}",
                    marketId,
                    marketName,
                    new ScheduleConstraint {Time = sessionClose},
                    new ScheduleConstraint {Time = nextSessionOpen},
                    assetRegex);
                
                result.Add(ss);
            }

            return result;
        }

        public static ScheduleSettings GetSingleSessionScheduleSettings(string marketId,
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
        
        public static List<ScheduleSettings> MapWeekends(string marketId, string marketName, List<DayOfWeek> weekends, string assetPairRegex)
        {
            var result = new List<ScheduleSettings>();
            
            foreach (var weekend in weekends)
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
                result.Add(ScheduleSettings.Create(id, marketId, marketName, start, end, assetPairRegex));
            }
            
            return result;
        }
    }
}