using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatorySettings
{
    public class GetAllBrokerRegulatorySettingsResponse
    {
        public IReadOnlyList<BrokerRegulatorySettingsContract> BrokerRegulatorySettings { get; set; }
    }
}