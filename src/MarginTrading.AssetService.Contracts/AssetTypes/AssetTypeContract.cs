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
        public Guid Id { get; set; }
        /// <summary>
        /// Id of the related regulatory type from MDM
        /// </summary>
        public Guid RegulatoryTypeId { get; set; }
        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        public string Name { get; set; }
    }
}