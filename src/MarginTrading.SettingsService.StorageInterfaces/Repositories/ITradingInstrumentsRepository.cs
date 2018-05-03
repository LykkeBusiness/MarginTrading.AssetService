using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Settings;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingInstrumentsRepository : IGenericCrudRepository<ITradingInstrument>
    {
        Task<IEnumerable<ITradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId,
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults);
    }
}
