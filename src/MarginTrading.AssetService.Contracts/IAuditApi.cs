using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Contracts.Common;
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
        Task<PaginatedResponseContract<AuditContract>> GetAuditTrailAsync([Query] GetAuditLogsRequest request, int? skip = null, int? take = null);
    }
}