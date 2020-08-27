using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IClientProfilesRepository
    {
        Task InsertAsync(ClientProfileWithTemplate model, IEnumerable<ClientProfileSettings> clientProfileSettingsToAdd);
        Task UpdateAsync(ClientProfile model);
        Task DeleteAsync(Guid id);
        Task<IReadOnlyList<ClientProfile>> GetAllAsync();
        Task<ClientProfile> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}