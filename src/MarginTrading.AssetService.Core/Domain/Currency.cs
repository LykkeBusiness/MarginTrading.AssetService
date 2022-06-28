using Lykke.Snow.Audit.Abstractions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class Currency : IAuditableObject<AuditDataType>
    {
        public string Id { get; set; }
        
        public string InterestRateMdsCode  { get; set; }

        public int Accuracy { get; set; } = 2;
        
        public byte[] Timestamp { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.Currency;

        public string GetAuditReference() => Id;
    }
}