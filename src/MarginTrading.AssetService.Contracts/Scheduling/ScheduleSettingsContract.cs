// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.Scheduling
{
    /// <summary>
    /// Start and End constraints must be of the same type.
    /// Types are described in ScheduleConstraintContract type description.
    /// </summary>
    public class ScheduleSettingsContract
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        public HashSet<string> AssetPairs { get; set; } = new HashSet<string>();
        public string MarketId { get; set; }
        public string MarketName { get; set; }

        public bool? IsTradeEnabled { get; set; } = false;
        public TimeSpan? PendingOrdersCutOff { get; set; }

        public ScheduleConstraintContract Start { get; set; }
        public ScheduleConstraintContract End { get; set; }
    }
}
