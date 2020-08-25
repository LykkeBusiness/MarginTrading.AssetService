using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Response model to get all client profile settings
    /// </summary>
    public class GetAllClientProfileSettingsResponse
    {
        /// <summary>
        /// Collection of settings
        /// </summary>
        public IReadOnlyList<ClientProfileSettingsContract> ClientProfileSettings { get; set; }
    }
}