using MessagePack;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Contract for client profile
    /// </summary>
    [MessagePackObject]
    public class ClientProfileContract
    {
        /// <summary>
        /// Id of the client profile
        /// </summary>
        [Key(0)]
        public string Id { get; set; }
        /// <summary>
        /// Id of the regulatory profile from MDM
        /// </summary>
        [Key(1)]
        public string RegulatoryProfileId { get; set; }
        /// <summary>
        /// Is this the default broker profile
        /// </summary>
        [Key(2)]
        public bool IsDefault { get; set; }
    }
}