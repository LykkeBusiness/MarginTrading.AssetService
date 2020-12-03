using System;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Contract for asset type
    /// </summary>
    public class AssetTypeContract
    {
        /// <summary>
        /// Id of asset type
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Id of the related regulatory type from MDM
        /// </summary>
        public string RegulatoryTypeId { get; set; }
        /// <summary>
        /// Id of the underlying category for the asset type
        /// </summary>
        public string UnderlyingCategoryId { get; set; }
    }
}