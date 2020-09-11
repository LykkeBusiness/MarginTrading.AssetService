using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface IClientProfileSettingsApi
    {
        /// <summary>
        /// Get client profile settings by ids
        /// </summary>
        /// <returns></returns>
        [Get("/api/client-profile-settings/profile/{profileId}/type/{typeId}")]
        Task<GetClientProfileSettingsByIdsResponse> GetClientProfileSettingsByIdsAsync(string profileId, string typeId);

        /// <summary>
        /// Get all client profile settings
        /// </summary>
        /// <returns></returns>
        [Get("/api/client-profile-settings")]
        Task<GetAllClientProfileSettingsResponse> GetClientProfileSettingsByRegulationAsync();

        /// <summary>
        /// Updates existing client settings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="profileId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [Put("/api/client-profile-settings/profile/{profileId}/type/{typeId}")]
        Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> UpdateClientProfileSettingsAsync(
            [Body] UpdateClientProfileSettingsRequest request, string profileId, string typeId);
    }
}