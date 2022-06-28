using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IAuditRepository
    {
        Task InsertAsync(IAuditModel<AuditDataType> model);

        Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take);
    }
}