using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IBrokerRegulatoryProfilesService
    {
        Task InsertAsync(BrokerRegulatoryProfileWithTemplate model, string username, string correlationId);

        Task UpdateAsync(BrokerRegulatoryProfile model, string username, string correlationId);

        Task DeleteAsync(Guid id, string username, string correlationId);

        Task<BrokerRegulatoryProfile> GetByIdAsync(Guid id);

        Task<IReadOnlyList<BrokerRegulatoryProfile>> GetAllAsync();
    }
}