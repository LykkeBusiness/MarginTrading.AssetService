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

        /// <summary>
        /// Creates new instance of <see cref="TradingDayInfo"/>
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="isTradingEnabled"></param>
        /// <param name="lastTradingDay"></param>
        /// <param name="isBusinessDay"></param>
        /// <param name="nextTradingDayStart"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
            
            // TradingDayInfo is used not only for trading day schedule,
            // but also for markets schedule inside the trading day.
            // In the latter case, the next trading day start can be equal to
            // the last trading day, therefore "<" but not "<=" comparison is used.
            if (new TradingDay(nextTradingDayStart) < lastTradingDay)
            {
                throw new ArgumentOutOfRangeException(nameof(nextTradingDayStart),
                    "Next trading day start cannot be less than last trading day");
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