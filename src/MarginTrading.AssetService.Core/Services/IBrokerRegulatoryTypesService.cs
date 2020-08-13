using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IBrokerRegulatoryTypesService
    {
        Task InsertAsync(BrokerRegulatoryTypeWithTemplate model, string username, string correlationId);
        Task UpdateAsync(BrokerRegulatoryType model, string username, string correlationId);
        Task DeleteAsync(Guid id, string username, string correlationId);
        Task<BrokerRegulatoryType> GetByIdAsync(Guid id);
        Task<IReadOnlyList<BrokerRegulatoryType>> GetAllAsync();
    }
}