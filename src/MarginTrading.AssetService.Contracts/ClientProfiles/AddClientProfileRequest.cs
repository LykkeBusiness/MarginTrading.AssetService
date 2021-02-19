using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Request to add client profile
    /// </summary>
    public class AddClientProfileRequest
    {
        /// <summary>
        /// Id of the related regulation
        /// </summary>
        [Required]
        public string RegulatoryProfileId { get; set; }

        /// <summary>
        /// Id of the client profile
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Is the new regulatory profile default
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Id of existing client profile which will be used as template for settings creation
        /// </summary>
        public string ClientProfileTemplateId { get; set; }
    }
}