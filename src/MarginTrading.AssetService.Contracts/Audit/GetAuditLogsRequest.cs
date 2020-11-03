using System;
using Refit;

namespace MarginTrading.AssetService.Contracts.Audit
{
    public class GetAuditLogsRequest
    {
        public string CorrelationId { get; set; }

        public string UserName { get; set; }

        public AuditEventType? ActionType { get; set; }

        [Query(CollectionFormat.Multi)]
        public AuditDataType[] DataTypes { get; set; }

        public string ReferenceId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}