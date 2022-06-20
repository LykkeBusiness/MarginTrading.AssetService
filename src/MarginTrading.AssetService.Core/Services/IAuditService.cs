using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAuditService
    {
        Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take);

        Task CreateAuditRecord(AuditEventType eventType,
            string userName,
            IAuditableObject<AuditDataType> current,
            IAuditableObject<AuditDataType> original = null);
    }
}