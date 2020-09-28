using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAssetPairService
    {
        Task<IAssetPair> GetByIdAsync(string assetPairId);
        Task<IReadOnlyList<IAssetPair>> GetByIdsAsync(IEnumerable<string> assetPairIds);
        Task<PaginatedResponse<IAssetPair>> GetPaginatedWithFilterAsync(string filter, int? skip, int? take);
        Task<IReadOnlyList<IAssetPair>> GetWithFilterAsync(string filter);
        Task<IAssetPair> ChangeSuspendStatusAsync(string assetPairId, bool status);
    }
}