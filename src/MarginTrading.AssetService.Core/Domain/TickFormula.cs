using System.Collections.Generic;
using Lykke.Snow.Audit.Abstractions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class TickFormula : ITickFormula, IAuditableObject<AuditDataType>
    {
        public string Id { get; set; }

        public List<decimal> PdlLadders { get; set; }

        public List<decimal> PdlTicks { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.TickFormula;

        public string GetAuditReference() => Id;
    }
}