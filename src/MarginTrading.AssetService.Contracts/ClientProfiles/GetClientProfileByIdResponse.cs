using Lykke.Contracts.Responses;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Response model to get client profile by id
    /// </summary>
    public class GetClientProfileByIdResponse : ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        /// <summary>
        /// Client profile model
        /// </summary>
        public ClientProfileContract ClientProfile { get; set; }
    }
}