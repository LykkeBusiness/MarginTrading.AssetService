using System;
using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class ScheduleSettings : IScheduleSettings
    {
        public ScheduleSettings(string id, int rank, string assetPairRegex, HashSet<string> assetPairs, string marketId,
            bool? isTradeEnabled, TimeSpan? pendingOrdersCutOff, ScheduleConstraint start, ScheduleConstraint end)
        {
            Id = id;
            Rank = rank;
            AssetPairRegex = assetPairRegex;
            AssetPairs = assetPairs;
            MarketId = marketId;
            IsTradeEnabled = isTradeEnabled;
            PendingOrdersCutOff = pendingOrdersCutOff;
            Start = start;
            End = end;
        }

        public string Id { get; }
        public int Rank { get; }
        public string AssetPairRegex { get; }
        public HashSet<string> AssetPairs { get; }
        public string MarketId { get; }

        public bool? IsTradeEnabled { get; }
        public TimeSpan? PendingOrdersCutOff { get; }

        public ScheduleConstraint Start { get; }
        public ScheduleConstraint End { get; }

        public override string ToString()
        {
            return $"Id: {Id}, IsTradeEnabled: {IsTradeEnabled}, Rank: {Rank}, MarketId: {MarketId}, Start: {Start}, End: {End}, AssetPairRegex: {AssetPairRegex}, AssetPairs: {string.Join(",", AssetPairs)}, PendingOrdersCutOff: {PendingOrdersCutOff}.";
        }
    }
}