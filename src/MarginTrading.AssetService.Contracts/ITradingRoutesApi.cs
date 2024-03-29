﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.Routes;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Trading route management
    /// </summary>
    [PublicAPI]
    public interface ITradingRoutesApi
    {
        /// <summary>
        /// Get the list of trading routes
        /// </summary>
        [Get("/api/routes/")]
        Task<List<MatchingEngineRouteContract>> List();
        
        /// <summary>
        /// Get the list of trading routes, with optional pagination
        /// </summary>
        [Get("/api/routes/by-pages")]
        Task<PaginatedResponse<MatchingEngineRouteContract>> ListByPages(
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);

        /// <summary>
        /// Create new trading route
        /// </summary>
        [Post("/api/routes/")]
        Task<MatchingEngineRouteContract> Insert([Body] MatchingEngineRouteContract route);

        /// <summary>
        /// Get the trading route
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/routes/{routeId}")]
        Task<MatchingEngineRouteContract> Get([NotNull] string routeId);

        /// <summary>
        /// Update the trading route
        /// </summary>
        [Put("/api/routes/{routeId}")]
        Task<MatchingEngineRouteContract> Update( [NotNull] string routeId, [Body] MatchingEngineRouteContract route);

        /// <summary>
        /// Delete the trading route
        /// </summary>
        [Delete("/api/routes/{routeId}")]
        Task Delete([NotNull] string routeId);
    }
}
