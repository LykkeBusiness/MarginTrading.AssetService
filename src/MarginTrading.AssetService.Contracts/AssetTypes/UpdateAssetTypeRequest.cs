namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Request model to update asset type
    /// </summary>
    public class UpdateAssetTypeRequest
    {
        /// <summary>
        /// Name of the asset type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        public string Username { get; set; }
    }
}