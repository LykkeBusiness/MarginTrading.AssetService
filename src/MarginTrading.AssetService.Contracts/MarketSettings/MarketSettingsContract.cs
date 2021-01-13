// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    /// <summary>
    /// Market settings contract
    /// </summary>
    [MessagePackObject]
    public class MarketSettingsContract
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key(0)]
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Key(1)]
        public string Name { get; set; }
        /// <summary>
        /// Dividends long
        /// </summary>
        [Key(2)]
        public decimal? DividendsLong { get; set; }
        /// <summary>
        /// Dividends short
        /// </summary>
        [Key(3)]
        public decimal? DividendsShort { get; set; }
        /// <summary>
        /// Dividends 871M
        /// </summary>
        [Key(4)]
        public decimal? Dividends871M { get; set; }
        /// <summary>
        /// When the trading day opens
        /// </summary>
        [Key(5)]
        public TimeSpan[] Open { get; set; }
        /// <summary>
        /// When the trading day closes
        /// </summary>
        [Key(6)]
        public TimeSpan[] Close { get; set; }
        /// <summary>
        /// Timezone
        /// </summary>
        [Key(7)]
        public string Timezone { get; set; }
        /// <summary>
        /// List of holidays
        /// </summary>
        [Key(8)]
        public List<DateTime> Holidays { get; set; }
        /// <summary>
        /// Market schedule settings
        /// </summary>
        [Key(9)]
        public MarketScheduleContract MarketSchedule { get; set; }
    }
}