using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class BrokerRegulatoryProfileWithTemplate : BrokerRegulatoryProfile
    {
        public Guid? BrokerRegulatoryProfileTemplateId { get; set; }
        public decimal MinMargin { get; set; }
        public bool IsAvailable { get; set; }
    }
}