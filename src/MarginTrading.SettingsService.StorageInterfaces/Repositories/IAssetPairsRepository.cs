using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Client.AssetPair;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetPairsRepository : IGenericCrudRepository<AssetPair>
    {
        Task<IReadOnlyList<AssetPair>> GetAsync(Func<AssetPair, bool> filter);
    }
}
