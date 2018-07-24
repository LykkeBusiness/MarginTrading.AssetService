using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
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

        public async Task<PaginatedResponse<ITradingRoute>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            var allData = await GetAsync();

            //TODO refactor before using azure impl
            var data = allData.OrderBy(x => x.Id).ToList();
            var filtered = take.HasValue ? data.Skip(skip ?? 0).Take(PaginationHelper.GetTake(take)).ToList() : data;
            
            return new PaginatedResponse<ITradingRoute>(
                contents: filtered,
                start: skip ?? 0,
                size: filtered.Count,
                totalSize: data.Count
            );
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