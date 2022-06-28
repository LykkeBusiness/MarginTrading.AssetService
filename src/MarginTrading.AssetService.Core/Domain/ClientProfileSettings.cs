using Lykke.Snow.Audit.Abstractions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfileSettings : IAuditableObject<AuditDataType>
    {
        public string RegulatoryProfileId { get; set; }
        public string RegulatoryTypeId { get; set; }
        public string ClientProfileId { get; set; }
        public string AssetTypeId { get; set; }
        public decimal Margin { get; set; }
        public decimal ExecutionFeesFloor { get; set; }
        public decimal ExecutionFeesCap { get; set; }
        public decimal ExecutionFeesRate { get; set; }
        public decimal FinancingFeesRate { get; set; }
        public decimal OnBehalfFee { get; set; }
        public bool IsAvailable { get; set; }
        public AuditDataType GetAuditDataType() => AuditDataType.ClientProfileSettings;
        public string GetAuditReference() => $"ClientProfileId:{ClientProfileId},AssetTypeId:{AssetTypeId}";
    }
}