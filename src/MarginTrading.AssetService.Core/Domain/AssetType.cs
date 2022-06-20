using Lykke.Snow.Audit.Abstractions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class AssetType : IAuditableObject<AuditDataType>
    {
        public string Id { get; set; }
        public string RegulatoryTypeId { get; set; }
        public string UnderlyingCategoryId { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.AssetType;

        public string GetAuditReference() => Id;
    }
}