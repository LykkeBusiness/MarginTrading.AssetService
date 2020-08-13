using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class BrokerRegulatorySettings
    {
        public Guid BrokerProfileId { get; set; }
        public string ProfileName { get; set; }
        public Guid BrokerTypeId { get; set; }
        public string TypeName { get; set; }
        public decimal MarginMin { get; set; }
        public decimal ExecutionFeesFloor { get; set; }
        public decimal ExecutionFeesCap { get; set; }
        public decimal ExecutionFeesRate { get; set; }
        public decimal FinancingFeesRate { get; set; }
        public decimal PhoneFees { get; set; }
        public bool IsAvailable { get; set; }
    }
}