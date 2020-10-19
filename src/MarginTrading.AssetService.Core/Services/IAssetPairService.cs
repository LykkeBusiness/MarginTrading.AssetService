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
        Task<IReadOnlyList<IAssetPair>> GetAllIncludingFxParisWithFilterAsync();
        Task<IAssetPair> ChangeSuspendStatusAsync(string assetPairId, bool status);
    }
}