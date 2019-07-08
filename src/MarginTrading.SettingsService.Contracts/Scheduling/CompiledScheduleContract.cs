// Copyright (c) 2019 Lykke Corp.

using System.Collections.Generic;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    public class CompiledScheduleContract
    {
        public string AssetPairId { get; set; }
        public List<CompiledScheduleSettingsContract> ScheduleSettings { get; set; }
    }
}
