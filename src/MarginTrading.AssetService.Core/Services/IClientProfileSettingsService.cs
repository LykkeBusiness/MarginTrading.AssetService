using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IClientProfileSettingsService
    {
        Task UpdateAsync(ClientProfileSettings model, string username, string correlationId);
        Task<ClientProfileSettings> GetByIdAsync(string profileId, string typeId);
        Task<List<ClientProfileSettings>> GetAllAsync();
        Task<bool> WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(RegulatorySettingsDto regulatorySettings);
    }
}