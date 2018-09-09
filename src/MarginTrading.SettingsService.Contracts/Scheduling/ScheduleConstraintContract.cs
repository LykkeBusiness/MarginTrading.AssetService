using System;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    /// <summary>
    /// 3 cases are valid: {Time}, {Date & Time}, {DayOfWeek & Time}
    /// </summary>
    public class ScheduleConstraintContract
    {
        public string Date { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeSpan Time { get; set; }

        public ScheduleConstraintTypeContract GetConstraintType()
        {
            var isDateCorrect = DateTime.TryParse(Date, out _);
            
            if (!isDateCorrect && DayOfWeek == default && Time != default)
            {
                return ScheduleConstraintTypeContract.Daily;
            }
            if (isDateCorrect && DayOfWeek == default && Time != default)
            {
                return ScheduleConstraintTypeContract.Single;
            }
            if (!isDateCorrect && DayOfWeek != default && Time != default)
            {
                return ScheduleConstraintTypeContract.Weekly;
            }
            //todo what about yearly?

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
