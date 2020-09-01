using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    public class AddMarketSettingsRequest
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// MIC code
        /// </summary>
        [Required]
        public string MICCode { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string Name { get; set; }
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
        public TimeSpan? Open { get; set; }
        /// <summary>
        /// When the trading day closes
        /// </summary>
        public TimeSpan? Close { get; set; }
        /// <summary>
        /// Timezone
        /// </summary>
        public string Timezone { get; set; }
        /// <summary>
        /// List of holidays for the broker
        /// </summary>
        public List<DateTime> Holidays { get; set; }
        /// <summary>
        /// Username of the user who adds the settings
        /// </summary>
        [Required]
        public string Username { get; set; }
    }
}