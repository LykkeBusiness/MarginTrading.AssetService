// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.AssetService.AzureRepositories.Entities;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.AzureRepositories.Repositories
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