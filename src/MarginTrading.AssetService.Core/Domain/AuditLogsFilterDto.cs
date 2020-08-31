using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class AuditLogsFilterDto
    {

        public string CorrelationId { get; set; }

        public string UserName { get; set; }

        public AuditEventType? ActionType { get; set; }

        public AuditDataType? DataType { get; set; }

        public string ReferenceId { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }
    }
}