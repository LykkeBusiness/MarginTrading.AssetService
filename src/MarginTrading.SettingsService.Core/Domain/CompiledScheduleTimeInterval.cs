// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class CompiledScheduleTimeInterval
    {
        public ScheduleSettings Schedule { get; }
        public DateTime Start { get; }
        public DateTime End { get; }

        public CompiledScheduleTimeInterval(ScheduleSettings schedule, DateTime start, DateTime end)
        {
            Schedule = schedule;
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"Start: {Start}, End: {End}, IsTradeEnabled: {Schedule?.IsTradeEnabled}, Rank: {Schedule?.Rank}";
        }
    }
}