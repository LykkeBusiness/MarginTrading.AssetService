using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class BrokerRegulatoryProfile
    {
        public Guid Id { get; set; }
        public Guid RegulatoryProfileId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}