using System.Threading.Tasks;
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
        /// <returns></returns>
        [Get("/api/audit")]
        Task<GetAuditLogsResponse> GetAuditTrailAsync([Query] GetAuditLogsRequest request);
    }
}