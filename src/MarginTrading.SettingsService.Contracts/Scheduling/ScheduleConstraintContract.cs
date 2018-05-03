using System;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    public class ScheduleConstraintContract
    {
        public string Date { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeSpan Time { get; set; }
    }
}
