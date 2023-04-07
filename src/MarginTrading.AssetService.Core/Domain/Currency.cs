using Lykke.Snow.Audit.Abstractions;

namespace MarginTrading.AssetService.Core.Domain
{
    public sealed class Currency : IAuditableObject<AuditDataType>
    {
        public static readonly ContractSize DefaultContractSize = 1;
        
        public string Id { get; set; }
        
        public string InterestRateMdsCode  { get; set; }

        public int Accuracy { get; set; } = 2;
        
        public byte[] Timestamp { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.Currency;

        public string GetAuditReference() => Id;

        public ContractSize ContractSize => DefaultContractSize;
    }
}