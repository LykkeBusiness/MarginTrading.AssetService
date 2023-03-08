using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Request model to update asset type
    /// </summary>
    public class UpdateAssetTypeRequest
    {
        /// <summary>
        /// Id of the related regulatory type
        /// </summary>
        [Required]
        public string RegulatoryTypeId { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Id of the underlying category for the asset type
        /// </summary>
        [Required]
        public string UnderlyingCategoryId { get; set; }
        
        public bool ExcludeSpreadFromProductCosts { get; set; }
    }
}