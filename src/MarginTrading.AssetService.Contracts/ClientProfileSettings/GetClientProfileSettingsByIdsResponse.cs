using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class GetClientProfileSettingsByIdsResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        public ClientProfileSettingsContract ClientProfileSettings { get; set; }
    }
}