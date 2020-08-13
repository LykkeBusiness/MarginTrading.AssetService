using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes
{
    public class GetAllBrokerRegulatoryTypesResponse
    {
        public IReadOnlyList<BrokerRegulatoryTypeContract> BrokerRegulatoryTypes { get; set; }
    }
}