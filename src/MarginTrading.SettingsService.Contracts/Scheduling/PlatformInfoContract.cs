using System;

namespace MarginTrading.SettingsService.Contracts.Scheduling
{
    /// <summary>
    /// Current platform trading info
    /// </summary>
    public class PlatformInfoContract
    {
        /// <summary>
        /// Trading day that was active last: current if enabled, interval.Start.Date if disabled
        /// </summary>
        public DateTime LastTradingDay { get; set; }
        
        /// <summary>
        /// Is trading enabled currently
        /// </summary>
        public bool IsTradingEnabled { get; set; }
    }
}