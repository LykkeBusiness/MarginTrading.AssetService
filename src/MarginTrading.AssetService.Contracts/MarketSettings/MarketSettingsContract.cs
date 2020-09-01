// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    /// <summary>
    /// Market settings contract
    /// </summary>
    public class MarketSettingsContract
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// MIC code
        /// </summary>
        public string MICCode { get; set; }
        /// <summary>
        /// Dividends long
        /// </summary>
        public decimal DividendsLong { get; set; }
        /// <summary>
        /// Dividends short
        /// </summary>
        public decimal DividendsShort { get; set; }
        /// <summary>
        /// Dividends 871M
        /// </summary>
        public decimal Dividends871M { get; set; }
        /// <summary>
        /// When the trading day opens
        /// </summary>
        public TimeSpan Open { get; set; }
        /// <summary>
        /// When the trading day closes
        /// </summary>
        public TimeSpan Close { get; set; }
        /// <summary>
        /// Timezone
        /// </summary>
        public string Timezone { get; set; }
        /// <summary>
        /// List of holidays
        /// </summary>
        public List<DateTime> Holidays { get; set; }
    }
}