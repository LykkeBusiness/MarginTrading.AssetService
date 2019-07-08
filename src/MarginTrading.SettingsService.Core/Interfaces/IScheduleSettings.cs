// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IScheduleSettings
    {
        string Id { get; }
        int Rank { get; }
        string AssetPairRegex { get; }
        HashSet<string> AssetPairs { get; }
        string MarketId { get; }

        bool? IsTradeEnabled { get; }
        TimeSpan? PendingOrdersCutOff { get; }

        ScheduleConstraint Start { get; }
        ScheduleConstraint End { get; }
   }
}
