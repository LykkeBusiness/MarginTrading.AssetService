// Copyright (c) 2019 Lykke Corp.

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
