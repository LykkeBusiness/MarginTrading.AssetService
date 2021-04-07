namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Response model to get default client profile settings
    /// </summary>
    public class GetDefaultClientProfileSettingsResponse
    {
        /// <summary>
        /// Client profile settings
        /// </summary>
        public ClientProfileSettingsContract ClientProfileSettings { get; set; }
    }
}