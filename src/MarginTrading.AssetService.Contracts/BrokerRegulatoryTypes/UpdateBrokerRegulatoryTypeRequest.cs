namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes
{
    /// <summary>
    /// Request model to update broker regulatory profile type
    /// </summary>
    public class UpdateBrokerRegulatoryTypeRequest
    {
        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        public string Username { get; set; }
    }
}