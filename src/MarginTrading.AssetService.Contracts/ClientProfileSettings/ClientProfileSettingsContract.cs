using System;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Broker regulatory settings contract
    /// </summary>
    public class ClientProfileSettingsContract
    {
        /// <summary>
        /// Id of the client profile
        /// </summary>
        public string ClientProfileId { get; set; }
        /// <summary>
        /// Id of the asset type
        /// </summary>
        public string AssetTypeId { get; set; }
        /// <summary>
        /// Margin rate
        /// </summary>
        public decimal Margin { get; set; }
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
        public decimal OnBehalfFee { get; set; }
        /// <summary>
        /// Sets the availability of asset type for trading
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}