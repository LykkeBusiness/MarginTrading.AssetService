using MessagePack;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    /// <summary>
    /// Broker regulatory settings contract
    /// </summary>
    [MessagePackObject]
    public class ClientProfileSettingsContract
    {
        /// <summary>
        /// Id of the client profile
        /// </summary>
        [Key(0)]
        public string ClientProfileId { get; set; }
        /// <summary>
        /// Id of the asset type
        /// </summary>
        [Key(1)]
        public string AssetTypeId { get; set; }
        /// <summary>
        /// Margin rate
        /// </summary>
        [Key(2)]
        public decimal Margin { get; set; }
        /// <summary>
        /// Execution fees floor value
        /// </summary>
        [Key(3)]
        public decimal ExecutionFeesFloor { get; set; }
        /// <summary>
        /// Execution fees cap value
        /// </summary>
        [Key(4)]
        public decimal ExecutionFeesCap { get; set; }
        /// <summary>
        /// Execution fees rate value
        /// </summary>
        [Key(5)]
        public decimal ExecutionFeesRate { get; set; }
        /// <summary>
        /// Financing fees rate value
        /// </summary>
        [Key(6)]
        public decimal FinancingFeesRate { get; set; }
        /// <summary>
        /// Phone fees value
        /// </summary>
        [Key(7)]
        public decimal OnBehalfFee { get; set; }
        /// <summary>
        /// Sets the availability of asset type for trading
        /// </summary>
        [Key(8)]
        public bool IsAvailable { get; set; }
        /// <summary>
        /// Id of the regulatory profile of the client profile
        /// </summary>
        [Key(9)]
        public string RegulatoryProfileId { get; set; }
        /// <summary>
        /// Id of the regulatory type of the asset type
        /// </summary>
        [Key(10)]
        public string RegulatoryTypeId { get; set; }
    }
}