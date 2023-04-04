using MessagePack;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Contract for asset type
    /// </summary>
    [MessagePackObject]
    public class AssetTypeContract
    {
        /// <summary>
        /// Id of asset type
        /// </summary>
        [Key(0)] 
        public string Id { get; set; }
        /// <summary>
        /// Id of the related regulatory type from MDM
        /// </summary>
        [Key(1)] 
        public string RegulatoryTypeId { get; set; }
        /// <summary>
        /// Id of the underlying category for the asset type
        /// </summary>
        [Key(2)] 
        public string UnderlyingCategoryId { get; set; }
        
        [Key(3)]
        public bool ExcludeSpreadFromProductCosts { get; set; }
    }
}