using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IBrokerRegulatoryProfilesRepository
    {
        Task InsertAsync(BrokerRegulatoryProfileWithTemplate model);
        Task UpdateAsync(BrokerRegulatoryProfile model);
        Task DeleteAsync(Guid id);
        Task<IReadOnlyList<BrokerRegulatoryProfile>> GetAllAsync();
        Task<BrokerRegulatoryProfile> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}