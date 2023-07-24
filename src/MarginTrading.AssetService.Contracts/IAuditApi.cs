using System.Threading.Tasks;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.Audit;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    public interface IAuditApi
    {
        /// <summary>
        /// Get audit logs
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Get("/api/audit")]
        Task<PaginatedResponse<AuditContract>> GetAuditTrailAsync([Query] GetAuditLogsRequest request, int? skip = null, int? take = null);
    }
}