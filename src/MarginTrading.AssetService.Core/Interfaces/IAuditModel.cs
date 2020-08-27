using System;

namespace MarginTrading.AssetService.Core.Interfaces
{
    public interface IAuditModel
    {
        DateTime Timestamp { get; set; }
        string CorrelationId { get; set; }
        string UserName { get; set; }
        string Type { get; set; }
        string DataType { get; set; }
        string DataReference { get; set; }
        string DataDiff { get; set; }
    }
}