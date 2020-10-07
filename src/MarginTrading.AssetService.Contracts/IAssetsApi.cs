// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Asset;
using MarginTrading.AssetService.Contracts.Common;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Assets management
    /// </summary>
    [PublicAPI]
    public interface IAssetsApi
    {
        /// <summary>
        /// Get the list of assets
        /// </summary>
        [Get("/api/assets")]
        Task<List<AssetContract>> List();

        /// <summary>
        /// Get the list of assets with optional pagination
        /// </summary>
        [Get("/api/assets/by-pages")]
        Task<PaginatedResponseContract<AssetContract>> ListByPages(
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);

        /// <summary>
        /// Get the asset
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/assets/{assetId}")]
        Task<AssetContract> Get([NotNull] string assetId);

        /// <summary>
        /// Get the list of assets
        /// </summary>
        [Get("/api/assets/legacy")]
        Task<List<Cronut.Dto.Assets.Asset>> GetLegacyAssets();
    }
}
