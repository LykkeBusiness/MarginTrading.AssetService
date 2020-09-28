﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
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

        public static ScheduleSettings Create(string marketId, ScheduleConstraint start, ScheduleConstraint end, string assetPairRegex)
        {
            return new ScheduleSettings(
                id: Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                rank: 0,
                assetPairRegex: assetPairRegex,
                assetPairs: new HashSet<string>(), 
                marketId: marketId,
                isTradeEnabled: false,
                pendingOrdersCutOff: TimeSpan.FromMilliseconds(0),
                start: start,
                end: end
            );
        }
    }
}