using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class BrokerRegulatoryTypeWithTemplate : BrokerRegulatoryType
    {
        public Guid? BrokerRegulatoryTypeTemplateId { get; set; }
        public decimal MinMargin { get; set; }
        public bool IsAvailable { get; set; }
    }
}