// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.Contracts.Scheduling
{
    /// <summary>
    /// 3 cases are valid: {Time}, {Date & Time}, {DayOfWeek & Time}
    /// </summary>
    public class ScheduleConstraintContract
    {
        public DateTime? Date { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeSpan Time { get; set; }

        public ScheduleConstraintTypeContract GetConstraintType()
        {
            if (Date == null && DayOfWeek == null)
            {
                return ScheduleConstraintTypeContract.Daily;
            }
            if (Date != null && DayOfWeek == null)
            {
                return ScheduleConstraintTypeContract.Single;
            }
            if (Date == null && DayOfWeek != null)
            {
                return ScheduleConstraintTypeContract.Weekly;
            }

            return ScheduleConstraintTypeContract.Invalid;
        }

        private static void Validate(ScheduleConstraintContract start, ScheduleConstraintContract end)
        {
            if (start == null || end == null)
            {
                throw new InvalidOperationException("Start and End must be set");
            }

            var startConstraintType = start.GetConstraintType();
            if (startConstraintType == ScheduleConstraintTypeContract.Invalid)
            {
                throw new InvalidOperationException("Start and End properties must be set according to one of 3 options");
            }

            if (startConstraintType != end.GetConstraintType())
            {
                throw new InvalidOperationException("Start and End properties must be set with the same type.");
            }

            // ReSharper disable once PossibleInvalidOperationException
            if (start.Date != null && start.Date.Value > end.Date.Value)
            {
                throw new InvalidOperationException("Start date cannot be after the End date.");
            }
        }

        public static void Validate(ScheduleSettingsContract contract)
        {
            Validate(contract.Start, contract.End);
        }

        public static void Validate(CompiledScheduleSettingsContract contract)
        {
            Validate(contract.Start, contract.End);
        }
    }
}
