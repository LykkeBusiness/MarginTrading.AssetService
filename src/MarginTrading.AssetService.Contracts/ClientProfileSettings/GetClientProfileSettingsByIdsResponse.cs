using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Response model to get client profile settings by ids
    /// </summary>
    public class GetClientProfileSettingsByIdsResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        /// <summary>
        /// Client profile settings
        /// </summary>
        public ClientProfileSettingsContract ClientProfileSettings { get; set; }
    }
}