using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class AssetPairsRepository : GenericAzureCrudRepository<IAssetPair, AssetPairEntity>, IAssetPairsRepository
    {
        public AssetPairsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "AssetPairs")
        {

        }

        public async Task<IReadOnlyList<IAssetPair>> GetAsync(params string[] assetPairIds)
        {
            return (assetPairIds.Length == 0
                ? await TableStorage.GetDataAsync()
                : await TableStorage.GetDataAsync(AssetPairEntity.Pk, x => assetPairIds.Contains(x.Id))).ToList();
        }

        public async Task<IAssetPair> GetByBaseAssetPairAsync(string baseAssetPairId)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, x => x.BaseAssetId == baseAssetPairId))
                .FirstOrDefault();
        }

        public async Task<IAssetPair> GetByBaseAssetPairAndNotByIdAsync(string id, string baseAssetPairId)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, 
                    x => x.Id != id && x.BaseAssetId == baseAssetPairId))
                .FirstOrDefault();
        }

        public async Task<IReadOnlyList<IAssetPair>> GetByLeAndMeModeAsync(string legalEntity = null, string matchingEngineMode = null)
        {
            return (await TableStorage.GetDataAsync(AssetPairEntity.Pk, 
                    x => (string.IsNullOrWhiteSpace(legalEntity) || x.LegalEntity == legalEntity)
                        && (string.IsNullOrWhiteSpace(matchingEngineMode) || x.MatchingEngineMode == matchingEngineMode)))
                .ToList();
        }

        public new async Task<IAssetPair> GetAsync(string assetPairId)
        {
            return await base.GetAsync(assetPairId, AssetPairEntity.Pk);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            await base.DeleteAsync(assetPairId);
        }

        public async Task UpdateAsync(IAssetPair obj)
        {
            await base.ReplaceAsync(obj);
        }
    }
}