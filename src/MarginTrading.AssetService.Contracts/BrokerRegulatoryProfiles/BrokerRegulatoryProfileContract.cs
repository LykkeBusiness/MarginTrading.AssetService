using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles
{
    /// <summary>
    /// Contract for broker regulatory profile
    /// </summary>
    public class BrokerRegulatoryProfileContract
    {
        /// <summary>
        /// Id of the broker regulatory profile
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Id of the regulatory profile from MDM
        /// </summary>
        public Guid RegulatoryProfileId { get; set; }
        /// <summary>
        /// Name of the broker regulatory profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is this the default broker profile
        /// </summary>
        public bool IsDefault { get; set; }
    }
}