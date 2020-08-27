using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Request model to update regulatory settings
    /// </summary>
    public class UpdateClientProfileSettingsRequest
    {
        /// <summary>
        /// Username of the user who is trying to update
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Minimum margin value
        /// </summary>
        [Required]
        public decimal Margin { get; set; }
        /// <summary>
        /// Execution fees floor value
        /// </summary>
        [Required]
        public decimal ExecutionFeesFloor { get; set; }
        /// <summary>
        /// Execution fees cap value
        /// </summary>
        [Required]
        public decimal ExecutionFeesCap { get; set; }
        /// <summary>
        /// Execution fees rate value
        /// </summary>
        [Required]
        public decimal ExecutionFeesRate { get; set; }
        /// <summary>
        /// Financing fees rate value
        /// </summary>
        [Required]
        public decimal FinancingFeesRate { get; set; }
        /// <summary>
        /// Phone fees value
        /// </summary>
        [Required]
        public decimal OnBehalfFee { get; set; }
        /// <summary>
        /// Are settings available
        /// </summary>
        [Required]
        public bool IsAvailable { get; set; }
    }
}