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
        [Get("/api/markets")]
        Task<List<MarketContract>> List();

        /// <summary>
        /// Get the market
        /// </summary>
        [Obsolete]
        [ItemCanBeNull]
        [Get("/api/markets/{marketId}")]
        Task<MarketContract> Get([NotNull] string marketId);
    }
}
