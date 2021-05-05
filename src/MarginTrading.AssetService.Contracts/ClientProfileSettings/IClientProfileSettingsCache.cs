using System;
using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public interface IClientProfileSettingsCache
    {
        void Start();
        
        void AddOrUpdate(ClientProfileSettingsContract clientProfileSettings);
        
        void Remove(ClientProfileSettingsContract clientProfileSettings);
        
        [Obsolete("Will be removed in future releases. Please use TryGetValue instead.")]
        ClientProfileSettingsContract GetByIds(string profileId, string assetType);

        bool TryGetValue(string profileId, string assetType, out ClientProfileSettingsContract result);

        IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false);
    }
}