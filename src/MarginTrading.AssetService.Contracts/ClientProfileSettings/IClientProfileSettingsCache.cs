using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public interface IClientProfileSettingsCache : IBaseCache<ClientProfileSettingsContract>
    {
        IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false);
    }
}