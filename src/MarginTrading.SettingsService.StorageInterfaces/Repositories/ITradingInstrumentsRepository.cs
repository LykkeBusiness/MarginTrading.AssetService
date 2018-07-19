using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Settings;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingInstrumentsRepository
    {
        Task<IReadOnlyList<ITradingInstrument>> GetAsync();
        Task<IReadOnlyList<ITradingInstrument>> GetByTradingConditionAsync(string tradingConditionId);
        Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null, 
            int? skip = null, int? take = null);
        Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId);
        Task<bool> TryInsertAsync(ITradingInstrument tradingInstrument);
        Task UpdateAsync(ITradingInstrument tradingInstrument);
        Task DeleteAsync(string assetPairId, string tradingConditionId);
        
        Task<IEnumerable<ITradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId,
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults);

    }
}
