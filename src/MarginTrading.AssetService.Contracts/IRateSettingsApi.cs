// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Rates;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Commission rate settings management.
    /// SettingsChangedEvent is generated on change.
    /// </summary>
    [PublicAPI]
    public interface IRateSettingsApi
    {
        /// <summary>
        /// Get overnight swap rates
        /// </summary>
        [Get("/api/rates/get-overnight-swap/{clientProfileId}")]
        Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync(string clientProfileId);

        /// <summary>
        /// Get overnight swap rates for asset pair id
        /// </summary>
        [Get("/api/rates/get-overnight-swap/{clientProfileId}/{assetPairId}")]
        Task<OvernightSwapRateContract> GetOvernightSwapRatesAsync(string clientProfileId, string assetPairId);

        /// <summary>	
        /// Get overnight swap rates for asset pair id	
        /// </summary>	
        [Post("/api/rates/get-overnight-swap/{clientProfileId}/list")]
        Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync(string clientProfileId, [Body] string[] assetPairIds);
    }
}