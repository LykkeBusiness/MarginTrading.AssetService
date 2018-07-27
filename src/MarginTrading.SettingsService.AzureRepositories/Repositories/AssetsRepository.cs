using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class AssetsRepository: GenericAzureCrudRepository<IAsset, AssetEntity>, IAssetsRepository
    {
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

        public async Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            var allData = await GetAsync();
            
            //TODO refactor before using azure impl
            var data = allData.OrderBy(x => x.Id).ToList();
            var filtered = take.HasValue ? data.Skip(skip ?? 0).Take(PaginationHelper.GetTake(take)).ToList() : data;
            
            return new PaginatedResponse<IAsset>(
                contents: filtered,
                start: skip ?? 0,
                size: filtered.Count,
                totalSize: data.Count
            );
        }
    }
}