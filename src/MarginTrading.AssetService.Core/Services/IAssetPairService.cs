using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAssetPairService
    {
        Task<IAssetPair> GetByIdAsync(string assetPairId);
        Task<IReadOnlyList<IAssetPair>> GetAllIncludingFxParisWithFilterAsync(IEnumerable<string> assetPairIds = null,
            bool onlyStarted = true);
    }
}