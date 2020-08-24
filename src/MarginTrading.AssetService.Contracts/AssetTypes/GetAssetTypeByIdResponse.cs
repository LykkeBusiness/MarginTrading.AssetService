using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    public class GetAssetTypeByIdResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        public AssetTypeContract AssetType { get; set; }
    }
}