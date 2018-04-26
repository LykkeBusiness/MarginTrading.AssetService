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
        string IScheduleSettings.AssetPairs => JsonConvert.SerializeObject(AssetPairs);
        public HashSet<string> AssetPairs { get; } = new HashSet<string>();
        public string MarketId { get; }

        public bool? IsTradeEnabled { get; } = false;
        string IScheduleSettings.PendingOrdersCutOff => PendingOrdersCutOff?.ToString();
        public TimeSpan? PendingOrdersCutOff { get; }

        string IScheduleSettings.Start => JsonConvert.SerializeObject(Start);
        public ScheduleConstraint Start { get; }
        string IScheduleSettings.End => JsonConvert.SerializeObject(End);
        public ScheduleConstraint End { get; }
    }
}