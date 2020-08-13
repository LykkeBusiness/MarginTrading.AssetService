namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles
{
    /// <summary>
    /// Request to update broker regulatory profile
    /// </summary>
    public class UpdateBrokerRegulatoryProfileRequest
    {
        /// <summary>
        /// Name of the regulatory profile
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Is the new regulatory profile default
        /// </summary>
        public bool IsDefault { get; set; }
    }
}