using System;
using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class ScheduleSettings : IScheduleSettings
    {
        public ScheduleSettings(string id, int rank, string assetPairRegex, string marketId, 
            TimeSpan? pendingOrdersCutOff, ScheduleConstraint start, ScheduleConstraint end)
        {
            Id = id;
            Rank = rank;
            AssetPairRegex = assetPairRegex;
            MarketId = marketId;
            PendingOrdersCutOff = pendingOrdersCutOff;
            Start = start;
            End = end;
        }

        public string Id { get; }
        public int Rank { get; }
        public string AssetPairRegex { get; }
        public HashSet<string> AssetPairs { get; } = new HashSet<string>();
        public string MarketId { get; }

        public bool? IsTradeEnabled { get; } = false;
        public TimeSpan? PendingOrdersCutOff { get; }

        public ScheduleConstraint Start { get; }
        public ScheduleConstraint End { get; }
    }
}