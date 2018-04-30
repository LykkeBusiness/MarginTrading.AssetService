using System;
using System.Collections.Generic;

namespace MarginTrading.SettingsService.Client.Scheduling
{
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
