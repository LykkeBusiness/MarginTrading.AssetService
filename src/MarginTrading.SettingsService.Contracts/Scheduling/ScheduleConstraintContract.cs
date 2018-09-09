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
            
            if (isDateCorrect && DayOfWeek == default && Time == default)
            {
                return ScheduleConstraintTypeContract.Daily;
            }
            if (isDateCorrect && DayOfWeek == default && Time != default)
            {
                return ScheduleConstraintTypeContract.Single;
            }
            if (!isDateCorrect && DayOfWeek != default && Time != default)
            {
                return ScheduleConstraintTypeContract.Recurring;
            }

            return ScheduleConstraintTypeContract.Invalid;
        } 
    }
}
