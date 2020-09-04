using System;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Interfaces
{
    public interface IAuditModel
    {
        int Id { get; set; }
        DateTime Timestamp { get; set; }
        string CorrelationId { get; set; }
        string UserName { get; set; }
        AuditEventType Type { get; set; }
        AuditDataType DataType { get; set; }
        string DataReference { get; set; }
        string DataDiff { get; set; }
    }
}