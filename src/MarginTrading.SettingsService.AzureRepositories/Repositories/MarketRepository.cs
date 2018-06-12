using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class MarketRepository : GenericAzureCrudRepository<IMarket, MarketEntity>, IMarketRepository
    {
        public MarketRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "Markets")
        {

        }

        public new async Task<IMarket> GetAsync(string marketId)
        {
            return await base.GetAsync(marketId, MarketEntity.Pk);
        }

        public async Task UpdateAsync(IMarket market)
        {
            await base.ReplaceAsync(market);
        }

        public async Task DeleteAsync(string marketId)
        {
            await base.DeleteAsync(marketId);
        }
    }
}