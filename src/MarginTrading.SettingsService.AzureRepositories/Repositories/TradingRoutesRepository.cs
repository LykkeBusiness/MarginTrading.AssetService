using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class TradingRoutesRepository : GenericAzureCrudRepository<ITradingRoute, TradingRouteEntity>,
        ITradingRoutesRepository
    {
        public TradingRoutesRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "TradingRoutes")
        {

        }

        public new async Task<ITradingRoute> GetAsync(string routeId)
        {
            return await base.GetAsync(routeId, TradingRouteEntity.Pk);
        }

        public async Task UpdateAsync(ITradingRoute tradingRoute)
        {
            await base.ReplaceAsync(tradingRoute);
        }

        public async Task DeleteAsync(string routeId)
        {
            await base.DeleteAsync(routeId);
        }
    }
}