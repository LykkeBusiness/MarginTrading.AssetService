using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatorySettings
{
    /// <summary>
    /// Broker regulatory settings contract
    /// </summary>
    public class BrokerRegulatorySettingsContract
    {
        /// <summary>
        /// Id of the broker regulatory profile
        /// </summary>
        public Guid BrokerProfileId { get; set; }
        /// <summary>
        /// Name of the broker regulatory profile
        /// </summary>
        public string BrokerProfileName { get; set; }
        /// <summary>
        /// Id of the broker regulatory type
        /// </summary>
        public Guid BrokerTypeId { get; set; }
        /// <summary>
        /// Name of the broker regulatory type
        /// </summary>
        public string BrokerTypeName { get; set; }
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