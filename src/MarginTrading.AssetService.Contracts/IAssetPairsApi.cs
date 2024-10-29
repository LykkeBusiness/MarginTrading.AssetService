// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.AssetPair;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Asset pairs management
    /// </summary>
    [PublicAPI]
    public interface IAssetPairsApi
    {
        /// <summary>
        /// Get asset pair by id
        /// </summary>
        [Get("/api/assetPairs/{assetPairId}")]
        [Obsolete("Use product api instead.")]
        Task<AssetPairContract> Get(string assetPairId);
        
        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode
        /// </summary>
        [Get("/api/assetPairs")]
        Task<List<AssetPairContract>> List();
        
        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        [Get("/api/assetPairs/by-pages")]
        [Obsolete("Use List since result from this method its not paginated anyway.")]
        Task<PaginatedResponse<AssetPairContract>> ListByPages(
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);
    }
}