using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetsRepository
    {
        Task<IReadOnlyList<IAsset>> GetAsync();
        Task<IAsset> GetAsync(string assetId);
        Task<bool> TryInsertAsync(IAsset asset);
        Task UpdateAsync(IAsset asset);
        Task DeleteAsync(string assetId);
        Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null);
    }
}
