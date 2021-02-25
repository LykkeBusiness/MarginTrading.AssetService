using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IClientProfileSettingsRepository
    {
        Task UpdateAsync(ClientProfileSettings model);
        Task<ClientProfileSettings> GetByIdsAsync(string profileId, string typeId);
        Task<List<ClientProfileSettings>> GetAllAsync(string clientProfileId, string assetTypeId);
        Task<bool> WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(RegulatorySettingsDto regulatorySettings);

        Task<List<ClientProfileSettings>> GetAllAsync(string clientProfileId,
            IEnumerable<string> assetTypeIds,
            bool availableOnly = false);

        Task<List<string>> GetActiveAssetTypeIdsAsync(bool defaultProfileOnly = false);
        Task<List<string>> GetActiveAssetTypeIdsAsync(string clientProfileId);
    }
}