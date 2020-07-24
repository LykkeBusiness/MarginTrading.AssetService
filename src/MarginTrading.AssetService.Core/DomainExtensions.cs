// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core
{
    public static class DomainExtensions
    {
        public static List<ScheduleSettings> WithRank(this IEnumerable<ScheduleSettings> scheduleSettings, int rank)
            => scheduleSettings.Select(scheduleSetting => new ScheduleSettings(
                scheduleSetting.Id,
                rank,
                scheduleSetting.AssetPairRegex,
                scheduleSetting.AssetPairs,
                scheduleSetting.MarketId,
                scheduleSetting.IsTradeEnabled,
                scheduleSetting.PendingOrdersCutOff,
                scheduleSetting.Start,
                scheduleSetting.End
            )).ToList();
        
        public static bool Enabled(this CompiledScheduleTimeInterval compiledScheduleTimeInterval)
        {
            return compiledScheduleTimeInterval?.Schedule.IsTradeEnabled ?? true;
        }
    }
}