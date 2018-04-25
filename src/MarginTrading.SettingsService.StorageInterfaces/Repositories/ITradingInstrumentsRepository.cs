using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingInstrumentsRepository : IGenericCrudRepository<TradingInstrument>
    {
        Task<IEnumerable<TradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId,
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults);
    }
}
