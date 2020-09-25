using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ITradingConditionsService
    {
        Task<IReadOnlyList<ITradingCondition>> GetAsync();
        Task<ITradingCondition> GetAsync(string tradingConditionId);
        Task<IReadOnlyList<ITradingCondition>> GetDefaultAsync();
    }
}