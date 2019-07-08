// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    public class CompiledScheduleSettingsContract
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public bool? IsTradeEnabled { get; set; } = false;
        public TimeSpan? PendingOrdersCutOff { get; set; }

        public ScheduleConstraintContract Start { get; set; }
        public ScheduleConstraintContract End { get; set; }
    }
}
