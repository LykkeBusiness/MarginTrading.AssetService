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
        Task DeleteAsync(Guid id);
        Task<IReadOnlyList<AssetType>> GetAllAsync();
        Task<IReadOnlyList<Guid>> GetAllIdsAsync();
        Task<AssetType> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}