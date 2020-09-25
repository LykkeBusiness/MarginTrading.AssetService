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
        /// Get order execution rates
        /// </summary>
        [Get("/api/rates/get-order-exec")]
        Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRatesAsync();

        /// <summary>
        /// Get order execution rates for asset pair id
        /// </summary>
        [Get("/api/rates/get-order-exec/{assetPairId}")]
        Task<OrderExecutionRateContract> GetOrderExecutionRateAsync(string assetPairId);

        /// <summary>
        /// Get overnight swap rates
        /// </summary>
        [Get("/api/rates/get-overnight-swap")]
        Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync();

        /// <summary>
        /// Get overnight swap rates for asset pair id
        /// </summary>
        [Get("/api/rates/get-overnight-swap/{assetPairId}")]
        Task<OvernightSwapRateContract> GetOvernightSwapRatesAsync(string assetPairId);

        /// <summary>
        /// Get on behalf rate
        /// </summary>
        [Get("/api/rates/get-on-behalf")]
        [ItemCanBeNull]
        Task<OnBehalfRateContract> GetOnBehalfRateAsync();
    }
}