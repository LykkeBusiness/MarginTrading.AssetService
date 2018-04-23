using System.Collections.Generic;

namespace MarginTrading.SettingsService.Client.Scheduling
{
    public class CompiledScheduleContract
    {
        public string AssetPairId { get; set; }
        public List<CompiledScheduleSettingsContract> ScheduleSettings { get; set; }
    }
}
