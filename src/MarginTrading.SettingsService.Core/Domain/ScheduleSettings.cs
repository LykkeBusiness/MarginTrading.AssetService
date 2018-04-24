using System;
using System.Collections.Generic;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class ScheduleSettings
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        public HashSet<string> AssetPairs { get; set; } = new HashSet<string>();
        public string MarketId { get; set; }

        public bool? IsTradeEnabled { get; set; } = false;
        public TimeSpan? PendingOrdersCutOff { get; set; }

        public ScheduleConstraint Start { get; set; }
        public ScheduleConstraint End { get; set; }
    }
}