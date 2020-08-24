using System;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Request to add asset type
    /// </summary>
    public class AddAssetTypeRequest
    {
        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Id of the related regulatory type from Mdm
        /// </summary>
        public Guid RegulatoryTypeId { get; set; }

        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of existing asset type which will be used as template for regulatory settings creation
        /// </summary>
        public Guid? AssetTypeTemplateId { get; set; }
    }
}