using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IAssetTypesService
    {
        Task InsertAsync(AssetTypeWithTemplate model, string username);
        Task UpdateAsync(AssetType model, string username);
        Task DeleteAsync(string id, string username);
        Task<AssetType> GetByIdAsync(string id);
        Task<IReadOnlyList<AssetType>> GetAllAsync();
        Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync(string regulatoryTypeId);
    }
}