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
    public class TradingConditionsRepository : GenericAzureCrudRepository<ITradingCondition, TradingConditionEntity>,
        ITradingConditionsRepository
    {
        public TradingConditionsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "TradingConditions")
        {

        }

        public async Task<IReadOnlyList<ITradingCondition>> GetDefaultAsync()
        {
            return (await TableStorage.GetDataAsync(x => x.IsDefault)).ToList();
        }

        public new async Task<ITradingCondition> GetAsync(string tradingConditionId)
        {
            return await base.GetAsync(tradingConditionId, TradingConditionEntity.Pk);
        }

        public async Task UpdateAsync(ITradingCondition tradingCondition)
        {
            await base.ReplaceAsync(tradingCondition);
        }
    }
}