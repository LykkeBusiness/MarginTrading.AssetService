﻿// Copyright (c) 2019 Lykke Corp.
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
        /// Get order execution rates for the list of asset pair ids
        /// </summary>
        /// <param name="assetPairIds">The list of asset pair ids</param>
        /// <returns></returns>
        [Post("/api/rates/get-order-exec/list")]
        Task<IReadOnlyList<OrderExecutionRateContract>> GetOrderExecutionRatesAsync(
            [Body] string[] assetPairIds);

        /// <summary>
        /// Insert or update existing order execution rates
        /// </summary>
        [Post("/api/rates/replace-order-exec")]
        Task ReplaceOrderExecutionRatesAsync([Body, NotNull] OrderExecutionRateContract[] rates);

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
        /// Get overnight swap rates for asset pair id
        /// </summary>
        [Post("/api/rates/get-overnight-swap/list")]
        Task<IReadOnlyList<OvernightSwapRateContract>> GetOvernightSwapRatesAsync([Body] string[] assetPairIds);

        /// <summary>
        /// Insert or update existing overnight swap rates
        /// </summary>
        [Post("/api/rates/replace-overnight-swap")]
        Task ReplaceOvernightSwapRatesAsync([Body, NotNull] OvernightSwapRateContract[] rates);

        /// <summary>
        /// Get on behalf rate
        /// </summary>
        [Get("/api/rates/get-on-behalf")]
        [ItemCanBeNull]
        Task<OnBehalfRateContract> GetOnBehalfRateAsync();

        /// <summary>
        /// Insert or update existing on behalf rate
        /// </summary>
        [Post("/api/rates/replace-on-behalf")]
        Task ReplaceOnBehalfRateAsync([Body, NotNull] OnBehalfRateContract rate);
    }
}