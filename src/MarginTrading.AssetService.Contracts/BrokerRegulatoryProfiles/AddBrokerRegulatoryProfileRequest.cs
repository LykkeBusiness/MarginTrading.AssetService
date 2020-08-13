using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles
{
    /// <summary>
    /// Request to add broker regulatory profile
    /// </summary>
    public class AddBrokerRegulatoryProfileRequest
    {
        /// <summary>
        /// Id of the related regulation
        /// </summary>
        public Guid RegulatoryProfileId { get; set; }

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

        /// <summary>
        /// Id of existing broker regulatory profile which will be used as template for regulatory settings creation
        /// </summary>
        public Guid? BrokerRegulatoryProfileTemplateId { get; set; }
    }
}