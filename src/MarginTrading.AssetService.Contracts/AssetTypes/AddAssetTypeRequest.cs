using System;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Id of the related regulatory type from Mdm
        /// </summary>
        [Required]
        public Guid RegulatoryTypeId { get; set; }

        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Id of existing asset type which will be used as template for regulatory settings creation
        /// </summary>
        public Guid? AssetTypeTemplateId { get; set; }
    }
}