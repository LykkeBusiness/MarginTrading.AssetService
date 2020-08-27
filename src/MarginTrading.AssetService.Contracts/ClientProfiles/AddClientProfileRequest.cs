using System;
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
        public Guid RegulatoryProfileId { get; set; }

        /// <summary>
        /// Name of the regulatory profile
        /// </summary>
        [Required]
        public string Name { get; set; }

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
        /// Id of existing broker regulatory profile which will be used as template for regulatory settings creation
        /// </summary>
        public Guid? ClientProfileTemplateId { get; set; }
    }
}