using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAuditService
    {
        Task<IReadOnlyList<IAuditModel>> GetAll(int? year, int? month);

        Task<bool> TryAudit(string correlationId, string userName, string referenceId,
            AuditDataType type, string newStateJson = null, string oldStateJson = null);
    }
}