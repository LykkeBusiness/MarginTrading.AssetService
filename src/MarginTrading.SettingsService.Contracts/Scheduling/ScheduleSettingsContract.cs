using System;
using System.Collections.Generic;

namespace MarginTrading.SettingsService.Contracts.Scheduling
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

        public bool? IsTradeEnabled { get; set; } = false;
        public TimeSpan? PendingOrdersCutOff { get; set; }

        public ScheduleConstraintContract Start { get; set; }
        public ScheduleConstraintContract End { get; set; }

        public void ValidateConstraints()
        {
            if (Start == null || End == null)
            {
                throw new InvalidOperationException("Start and End must be set");
            }

            var startConstraintType = Start.GetConstraintType();
            if (startConstraintType == ScheduleConstraintTypeContract.Invalid)
            {
                throw new InvalidOperationException("Start and End properties must be set according to one of 3 options");
            }

            if (startConstraintType != End.GetConstraintType())
            {
                throw new InvalidOperationException("Start and End properties must be set with the same type.");
            }
        }
    }
}
