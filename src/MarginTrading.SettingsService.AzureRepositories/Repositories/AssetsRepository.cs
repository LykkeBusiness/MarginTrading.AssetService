using AzureStorage;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class AssetsRepository: GenericAzureCrudRepository<Asset, AssetEntity>, IAssetsRepository
    {
        public AssetsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "Assets")
        {

        }
    }
}