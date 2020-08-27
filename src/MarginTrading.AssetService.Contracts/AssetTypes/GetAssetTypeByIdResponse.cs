using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Response model to get AssetType by ID
    /// </summary>
    public class GetAssetTypeByIdResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        /// <summary>
        /// Asset type model
        /// </summary>
        public AssetTypeContract AssetType { get; set; }
    }
}