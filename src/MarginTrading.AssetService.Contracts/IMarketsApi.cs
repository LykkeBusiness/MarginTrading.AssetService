// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Market;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Markets management
    /// </summary>
    [PublicAPI]
    public interface IMarketsApi
    {
        /// <summary>
        /// Get the list of Markets
        /// </summary>
        [Obsolete]
        [Get("/api/markets")]
        Task<List<MarketContract>> List();

        /// <summary>
        /// Create new market
        /// </summary>
        [Obsolete]
        [Post("/api/markets")]
        Task<MarketContract> Insert([Body] MarketContract market);

        /// <summary>
        /// Get the market
        /// </summary>
        [Obsolete]
        [ItemCanBeNull]
        [Get("/api/markets/{marketId}")]
        Task<MarketContract> Get([NotNull] string marketId);

        /// <summary>
        /// Update the market
        /// </summary>
        [Obsolete]
        [Put("/api/markets/{marketId}")]
        Task<MarketContract> Update([NotNull] string marketId, [Body] MarketContract market);

        /// <summary>
        /// Delete the market
        /// </summary>
        [Obsolete]
        [Delete("/api/markets/{marketId}")]
        Task Delete([NotNull] string marketId);
    }
}
