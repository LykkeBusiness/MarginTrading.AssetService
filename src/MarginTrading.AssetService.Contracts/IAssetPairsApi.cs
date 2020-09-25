// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;
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
        /// Get the list of asset pairs based on legal entity and matching engine mode
        /// </summary>
        [Get("/api/assetPairs")]
        [Obsolete("Use paginated action")]
        Task<List<AssetPairContract>> List(
            [Query] [CanBeNull] string legalEntity = null,
            [Query] [CanBeNull] MatchingEngineModeContract? matchingEngineMode = null, 
            [Query] [CanBeNull] string filter = null);
        
        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        [Get("/api/assetPairs/by-pages")]
        Task<PaginatedResponseContract<AssetPairContract>> ListByPages(
            [Query, CanBeNull] string legalEntity = null,
            [Query, CanBeNull] MatchingEngineModeContract? matchingEngineMode = null,
            [Query] [CanBeNull] string filter = null,
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);

        [ItemCanBeNull]
        [Get("/api/assetPairs/{assetPairId}")]
        Task<AssetPairContract> Get([NotNull] string assetPairId);
    }
}