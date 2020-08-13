using System;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes
{
    /// <summary>
    /// Request to add broker regulatory type
    /// </summary>
    public class AddBrokerRegulatoryTypeRequest
    {
        public string Username { get; set; }
        /// <summary>
        /// Id of the related regulatory type from Mdm
        /// </summary>
        public Guid RegulatoryTypeId { get; set; }

        /// <summary>
        /// Name of the regulatory type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the user who sent the request
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Id of existing regulatory type which will be used as template for regulatory settings creation
        /// </summary>
        public Guid? BrokerRegulatoryTypeTemplateId { get; set; }
    }
}