using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class AssetsRepository: GenericAzureCrudRepository<IAsset, AssetEntity>, IAssetsRepository
    {
        private IAssetsRepository _assetsRepositoryImplementation;

        public AssetsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "Assets")
        {

        }

        public new async Task<IAsset> GetAsync(string assetId)
        {
            return await base.GetAsync(assetId, AssetEntity.Pk);
        }

        public async Task UpdateAsync(IAsset asset)
        {
            await base.ReplaceAsync(asset);
        }

        public async Task DeleteAsync(string assetId)
        {
            await base.DeleteAsync(assetId);
        }
    }
}