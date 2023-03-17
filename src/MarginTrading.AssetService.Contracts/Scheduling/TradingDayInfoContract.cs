// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using Lykke.Snow.Common.TradingDays;

using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.Scheduling
{
    /// <summary>
    /// Current trading day info
    /// </summary>
    public class TradingDayInfoContract
    {
        /// <summary>
        /// Trading day that was active last: current if enabled, interval.Start.Date if disabled
        /// </summary>
        [JsonConverter(typeof(TradingDayConverter))]
        public TradingDay LastTradingDay { get; set; }
        
        /// <summary>
        /// Is trading enabled currently
        /// </summary>
        public bool IsTradingEnabled { get; set; }        
        
        public bool IsBusinessDay { get; set; }
        
        /// <summary>
        ///  Timestamp when next trading day with start
        /// </summary>
        public DateTime NextTradingDayStart { get; set; }
    }
}