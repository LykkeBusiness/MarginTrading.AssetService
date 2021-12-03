using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IClientProfilesService
    {
        Task InsertAsync(ClientProfileWithTemplate model, string username);

        Task UpdateAsync(ClientProfile model, string username);

        Task DeleteAsync(string id, string username);

        Task<ClientProfile> GetByIdAsync(string id);

        Task<IReadOnlyList<ClientProfile>> GetAllAsync();

        Task<ClientProfile> GetDefaultAsync();

        Task<bool> IsRegulatoryProfileAssignedToAnyClientProfileAsync(string regulatoryProfileId);
    }
}