using System;
using System.Collections.Generic;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    /// <summary>
    /// Start and End constraints must be in the same format: Date or DayOfWeek must be set in both of them.
    /// </summary>
    public class ScheduleSettingsContract
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        public HashSet<string> AssetPairs { get; set; } = new HashSet<string>();
        public string MarketId { get; set; }

        public bool? IsTradeEnabled { get; set; } = false;
        public TimeSpan? PendingOrdersCutOff { get; set; }

        public ScheduleConstraintContract Start { get; set; }
        public ScheduleConstraintContract End { get; set; }
    }
}
