using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IClientProfilesService
    {
        Task InsertAsync(ClientProfileWithTemplate model, string username, string correlationId);

        Task UpdateAsync(ClientProfile model, string username, string correlationId);

        Task DeleteAsync(string id, string username, string correlationId);

        Task<ClientProfile> GetByIdAsync(string id);

        Task<IReadOnlyList<ClientProfile>> GetAllAsync();
    }
}