using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class HalfWorkingDayExtensions
    {
        public static bool CanHaveScheduleConstraint(this WorkingDay day)
        {
            return day.Duration != WorkingDayDuration.WholeDay;
        }
        
        public static ScheduleConstraint GetStartScheduleConstraint(this WorkingDay day)
        {
            if (day.Duration == WorkingDayDuration.PartialDayBeforeMilestone)
            {
                return new ScheduleConstraint {Date = day.Timestamp.Date, Time = day.Timestamp.TimeOfDay};
            }

            if (day.Duration == WorkingDayDuration.PartialDayAfterMilestone)
            {
                return new ScheduleConstraint {Date = day.Timestamp.Date, Time = TimeSpan.Zero};
            }

            throw new ArgumentOutOfRangeException(nameof(day.Duration), $"Unexpected duration value: {day.Duration.ToString()}");
        }

        public static ScheduleConstraint GetEndScheduleConstraint(this WorkingDay day)
        {
            if (day.Duration == WorkingDayDuration.PartialDayBeforeMilestone)
            {
                return new ScheduleConstraint {Date = day.Timestamp.Date.AddDays(1), Time = TimeSpan.Zero};
            }

            if (day.Duration == WorkingDayDuration.PartialDayAfterMilestone)
            {
                return new ScheduleConstraint {Date = day.Timestamp.Date, Time = day.Timestamp.TimeOfDay};
            }
            
            throw new ArgumentOutOfRangeException(nameof(day.Duration), $"Unexpected duration value: {day.Duration.ToString()}");
        }

        public static string GetScheduleId(this WorkingDay day, string marketId)
        {
            return $"{marketId}_half_working_day_{day.Timestamp.Date}";
        }

        public static IReadOnlyList<ScheduleSettings> GetScheduleSettings(this IEnumerable<WorkingDay> days,
            string marketId,
            string marketName,
            string assetRegex)
        {
            return days
                .Where(d => d.CanHaveScheduleConstraint())
                .Select(d => ScheduleSettings.Create(
                    d.GetScheduleId(marketId),
                    marketId,
                    marketName,
                    d.GetStartScheduleConstraint(),
                    d.GetEndScheduleConstraint(),
                    assetRegex))
                .ToList();
        }
    }
}