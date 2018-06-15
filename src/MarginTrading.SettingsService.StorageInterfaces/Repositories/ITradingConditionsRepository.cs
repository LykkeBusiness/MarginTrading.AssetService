using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingConditionsRepository
    {
        Task<IReadOnlyList<ITradingCondition>> GetAsync();
        Task<ITradingCondition> GetAsync(string tradingConditionId);
        Task<IReadOnlyList<ITradingCondition>> GetDefaultAsync();
        Task<bool> TryInsertAsync(ITradingCondition tradingCondition);
        Task UpdateAsync(ITradingCondition tradingCondition);
    }
}
