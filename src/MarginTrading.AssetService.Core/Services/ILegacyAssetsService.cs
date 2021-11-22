using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.LegacyAsset;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ILegacyAssetsService
    {
        Task<List<Asset>> GetLegacyAssets(IEnumerable<string> productIds = null, bool startedOnly = true);
    }
}