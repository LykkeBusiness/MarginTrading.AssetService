using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Response model to get all asset types
    /// </summary>
    public class GetAllAssetTypesResponse
    {
        /// <summary>
        /// Collection of asset types
        /// </summary>
        public IReadOnlyList<AssetTypeContract> AssetTypes { get; set; }
    }
}