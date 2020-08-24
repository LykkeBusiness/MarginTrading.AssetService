using System;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Contract for client profile
    /// </summary>
    public class ClientProfileContract
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
        /// Name of the client profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is this the default broker profile
        /// </summary>
        public bool IsDefault { get; set; }
    }
}