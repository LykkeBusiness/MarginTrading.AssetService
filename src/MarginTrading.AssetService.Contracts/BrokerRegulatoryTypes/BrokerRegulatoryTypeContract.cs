using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes
{
    /// <summary>
    /// Contract for broker regulatory type
    /// </summary>
    public class BrokerRegulatoryTypeContract
    {
        /// <summary>
        /// Id of the broker regulatory type
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Id of the related regulatory type from MDM
        /// </summary>
        public Guid RegulatoryTypeId { get; set; }
        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        public string Name { get; set; }
    }
}