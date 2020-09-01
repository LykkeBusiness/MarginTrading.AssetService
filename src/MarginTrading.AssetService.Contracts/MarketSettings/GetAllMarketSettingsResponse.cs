using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    public class GetAllMarketSettingsResponse
    {
        /// <summary>
        /// Collection of market settings
        /// </summary>
        public IReadOnlyList<MarketSettingsContract> MarketSettingsContracts { get; set; }
    }
}