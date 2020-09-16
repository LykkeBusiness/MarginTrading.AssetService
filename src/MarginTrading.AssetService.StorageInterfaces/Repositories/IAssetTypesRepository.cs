using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IAssetTypesRepository
    {
        Task InsertAsync(AssetTypeWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd);
        Task UpdateAsync(AssetType model);
        Task DeleteAsync(string id);
        Task<IReadOnlyList<AssetType>> GetAllAsync();
        Task<IReadOnlyList<string>> GetAllIdsAsync();
        Task<AssetType> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync(string regulatoryTypeId);
    }
}