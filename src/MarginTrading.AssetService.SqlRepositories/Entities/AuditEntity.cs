using System;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class AuditEntity : IAuditModel<AuditDataType>
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string CorrelationId { get; set; }

        public string UserName { get; set; }

        public AuditEventType Type { get; set; }

        public AuditDataType DataType { get; set; }

        public string DataReference { get; set; }

        public string DataDiff { get; set; }

        public static AuditEntity Create(IAuditModel<AuditDataType> model)
        {
            return new AuditEntity
            {
                CorrelationId = model.CorrelationId,
                Type = model.Type,
                DataReference = model.DataReference,
                UserName = model.UserName,
                Timestamp = model.Timestamp,
                DataType = model.DataType,
                DataDiff = model.DataDiff
            };
        }
    }
}