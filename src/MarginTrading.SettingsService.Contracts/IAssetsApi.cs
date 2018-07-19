using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Asset;
using MarginTrading.SettingsService.Contracts.Common;
using Refit;

namespace MarginTrading.SettingsService.Contracts
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
        /// Create new asset
        /// </summary>
        [Post("/api/assets")]
        Task<AssetContract> Insert([Body] AssetContract asset);

        /// <summary>
        /// Get the asset
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/assets/{assetId}")]
        Task<AssetContract> Get([NotNull] string assetId);

        /// <summary>
        /// Update the asset
        /// </summary>
        [Put("/api/assets/{assetId}")]
        Task<AssetContract> Update([NotNull] string assetId, [Body] AssetContract asset);

        /// <summary>
        /// Delete the asset
        /// </summary>
        [Delete("/api/assets/{assetId}")]
        Task Delete([NotNull] string assetId);
    }
}
