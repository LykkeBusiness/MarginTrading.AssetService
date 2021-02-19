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
        public string RegulatoryTypeId { get; set; }

        /// <summary>
        /// Id of the asset type
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Id { get; set; }

        /// <summary>
        /// Id of existing asset type which will be used as template for regulatory settings creation
        /// </summary>
        public string AssetTypeTemplateId { get; set; }

        /// <summary>
        /// Id of the underlying category for the asset type
        /// </summary>
        [Required]
        public string UnderlyingCategoryId { get; set; }
    }
}