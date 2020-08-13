using System;

namespace MarginTrading.AssetService.Contracts.Audit
{
    public class AuditContract
    {
        public DateTime Timestamp { get; set; }

        public string CorrelationId { get; set; }

        public string UserName { get; set; }

        public string Type { get; set; }

        public string DataType { get; set; }

        public string DataReference { get; set; }

        public string DataDiff { get; set; }
    }
}
