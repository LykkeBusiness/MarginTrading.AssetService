using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class CheckRegulationConstraintViolationRequest
    {
        /// <summary>
        /// Regulatory profile id
        /// </summary>
        [Required]
        public string RegulatoryProfileId { get; set; }

        /// <summary>
        /// Regulatory type id
        /// </summary>
        [Required]
        public string RegulatoryTypeId { get; set; }

        /// <summary>
        /// Minimum margin
        /// </summary>
        [Required]
        public decimal MarginMin { get; set; }

        /// <summary>
        /// Is regulation available
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }
    }
}