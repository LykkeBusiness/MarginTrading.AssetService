// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.Scheduling
{
    public class CompiledScheduleContract
    {
        public string AssetPairId { get; set; }
        public List<CompiledScheduleSettingsContract> ScheduleSettings { get; set; }
    }
}
