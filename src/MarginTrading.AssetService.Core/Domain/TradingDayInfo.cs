// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using Lykke.Snow.Common.TradingDays;

namespace MarginTrading.AssetService.Core.Domain
{
    // Trading day information
    public readonly struct TradingDayInfo
    {
        /// <summary>
        /// The timestamp information is actual for
        /// </summary>
        private DateTime Timestamp { get; }

        /// <summary>
        /// If trading is enabled at the moment of status creation
        /// </summary>
        public bool IsTradingEnabled { get; }

        /// <summary>
        /// Trading day that was active last (current day if enabled) at the moment of status creation
        /// </summary>
        public TradingDay LastTradingDay { get; }

        /// <summary>
        /// Indicates if the <see cref="LastTradingDay"/> day is business day
        /// </summary>
        public bool IsBusinessDay { get; }

        /// <summary>
        /// Timestamp when next trading day starts
        /// </summary>
        public DateTime NextTradingDayStart { get; }

        public TradingDayInfo(DateTime timestamp,
            bool isTradingEnabled,
            TradingDay lastTradingDay,
            bool isBusinessDay,
            DateTime nextTradingDayStart)
        {
            if (isTradingEnabled && lastTradingDay != new TradingDay(timestamp))
            {
                throw new ArgumentOutOfRangeException(nameof(lastTradingDay),
                    "Last trading day must be equal to timestamp date if trading is enabled");
            }

            if (new TradingDay(timestamp) < lastTradingDay)
            {
                throw new ArgumentOutOfRangeException(nameof(lastTradingDay),
                    "Last trading day cannot be greater than timestamp date");
            }
            
            if (new TradingDay(nextTradingDayStart) <= lastTradingDay)
            {
                throw new ArgumentOutOfRangeException(nameof(nextTradingDayStart),
                    "Next trading day start cannot be less than or equal to last trading day");
            }

            if (nextTradingDayStart <= timestamp)
            {
                throw new ArgumentOutOfRangeException(nameof(nextTradingDayStart),
                    "Next trading day start cannot be less than timestamp");
            }

            Timestamp = timestamp;
            IsTradingEnabled = isTradingEnabled;
            LastTradingDay = lastTradingDay;
            IsBusinessDay = isBusinessDay;
            NextTradingDayStart = nextTradingDayStart;
        }
    }
}