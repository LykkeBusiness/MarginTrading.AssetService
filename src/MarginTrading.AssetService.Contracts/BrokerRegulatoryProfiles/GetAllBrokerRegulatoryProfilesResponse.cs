using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles
{
    public class GetAllBrokerRegulatoryProfilesResponse
    {
        public IReadOnlyList<BrokerRegulatoryProfileContract> BrokerRegulatoryProfiles { get; set; }
    }
}