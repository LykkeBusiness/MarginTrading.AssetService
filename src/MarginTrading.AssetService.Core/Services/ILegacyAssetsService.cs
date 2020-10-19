using System.Collections.Generic;
using System.Threading.Tasks;
using Cronut.Dto.Assets;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ILegacyAssetsService
    {
        Task<List<Asset>> GetLegacyAssets(IEnumerable<string> productIds = null);
    }
}