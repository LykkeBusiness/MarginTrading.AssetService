using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfileSettings
    {
        public Guid RegulatoryProfileId { get; set; }
        public Guid RegulatoryTypeId { get; set; }
        public Guid ClientProfileId { get; set; }
        public string ClientProfileName { get; set; }
        public Guid AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public decimal MarginMin { get; set; }
        public decimal ExecutionFeesFloor { get; set; }
        public decimal ExecutionFeesCap { get; set; }
        public decimal ExecutionFeesRate { get; set; }
        public decimal FinancingFeesRate { get; set; }
        public decimal PhoneFees { get; set; }
        public bool IsAvailable { get; set; }
    }
}