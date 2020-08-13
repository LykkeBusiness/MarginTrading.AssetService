using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatorySettings
{
    /// <summary>
    /// Request model to update regulatory settings
    /// </summary>
    public class UpdateBrokerRegulatorySettingsRequest
    {
        /// <summary>
        /// Username of the user who is trying to update
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Minimum margin value
        /// </summary>
        public decimal MarginMin { get; set; }
        /// <summary>
        /// Execution fees floor value
        /// </summary>
        public decimal ExecutionFeesFloor { get; set; }
        /// <summary>
        /// Execution fees cap value
        /// </summary>
        public decimal ExecutionFeesCap { get; set; }
        /// <summary>
        /// Execution fees rate value
        /// </summary>
        public decimal ExecutionFeesRate { get; set; }
        /// <summary>
        /// Financing fees rate value
        /// </summary>
        public decimal FinancingFeesRate { get; set; }
        /// <summary>
        /// Phone fees value
        /// </summary>
        public decimal PhoneFees { get; set; }
        /// <summary>
        /// Are settings available
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}