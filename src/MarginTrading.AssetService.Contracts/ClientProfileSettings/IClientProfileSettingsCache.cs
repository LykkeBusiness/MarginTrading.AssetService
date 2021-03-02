using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public interface IClientProfileSettingsCache
    {
        void Start();
        void AddOrUpdate(ClientProfileSettingsContract clientProfileSettings);
        void Remove(ClientProfileSettingsContract clientProfileSettings);
        ClientProfileSettingsContract GetByIds(string profileId, string assetType);

        IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false);
    }
}