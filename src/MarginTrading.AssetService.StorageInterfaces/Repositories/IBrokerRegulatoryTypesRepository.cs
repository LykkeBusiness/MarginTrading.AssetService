using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IBrokerRegulatoryTypesRepository
    {
        Task InsertAsync(BrokerRegulatoryTypeWithTemplate model);
        Task UpdateAsync(BrokerRegulatoryType model);
        Task DeleteAsync(Guid id);
        Task<IReadOnlyList<BrokerRegulatoryType>> GetAllAsync();
        Task<BrokerRegulatoryType> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}