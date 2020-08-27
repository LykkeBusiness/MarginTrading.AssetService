using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Request to update client profile
    /// </summary>
    public class UpdateClientProfileRequest
    {
        /// <summary>
        /// Name of the client profile
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Is the client profile default
        /// </summary>
        public bool IsDefault { get; set; }
    }
}