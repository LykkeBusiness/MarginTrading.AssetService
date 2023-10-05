using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public interface IClientProfileSettingsCache : IBaseCache<ClientProfileSettingsContract>
    {
        bool TryGetValue(string clientProfileId, string assetTypeId, out ClientProfileSettingsContract result);
        IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false);
    }
}