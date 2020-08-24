using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    public class GetClientProfileByIdResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        public ClientProfileContract ClientProfile { get; set; }
    }
}