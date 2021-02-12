using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ITradingInstrumentsService
    {
        Task<IReadOnlyList<ITradingInstrument>> GetAsync(string tradingConditionId);

        Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null, int? skip = null, int? take = null);

        Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId);
    }
}