using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAuditService
    {
        Task<PaginatedResponse<IAuditModel>> GetAll(AuditLogsFilterDto filter, int? skip, int? take);

        Task<bool> TryAudit(string correlationId, string userName, string referenceId,
            AuditDataType type, string newStateJson = null, string oldStateJson = null);
    }
}