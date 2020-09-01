// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

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