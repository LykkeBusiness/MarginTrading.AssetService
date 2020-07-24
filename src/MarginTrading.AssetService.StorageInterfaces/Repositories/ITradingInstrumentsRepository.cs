// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
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

        Task<List<ITradingInstrument>> UpdateBatchAsync(string tradingConditionId, List<TradingInstrument> items);
    }
}
