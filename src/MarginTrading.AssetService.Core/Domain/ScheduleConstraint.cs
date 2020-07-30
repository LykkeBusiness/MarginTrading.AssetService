// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;

namespace MarginTrading.AssetService.Core.Domain
{
    [UsedImplicitly]
    public class ScheduleConstraint : IEquatable<ScheduleConstraint>
    {
        public DateTime? Date { get; set; }
        public DayOfWeek? DayOfWeek { get; set; } 
        public TimeSpan Time { get; set; }
        
        public ScheduleConstraintType GetConstraintType()
        {
            if (Date == null && DayOfWeek == null)
            {
                return ScheduleConstraintType.Daily;
            }
            if (Date != null && DayOfWeek == null)
            {
                return ScheduleConstraintType.Single;
            }
            if (Date == null && DayOfWeek != null)
            {
                return ScheduleConstraintType.Weekly;
            }

            return ScheduleConstraintType.Invalid;
        }

        public bool Equals(ScheduleConstraint other)
        {
            return other != null
                   && Date == other.Date
                   && DayOfWeek == other.DayOfWeek
                   && Time == other.Time;
        }

        public override string ToString()
        {
            return $"Date: {Date}, Day of week: {DayOfWeek}, Time: {Time}";
        }
    }
}