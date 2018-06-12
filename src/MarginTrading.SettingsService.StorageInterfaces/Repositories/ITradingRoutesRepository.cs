using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingRoutesRepository
    {
        Task<IReadOnlyList<ITradingRoute>> GetAsync();
        Task<ITradingRoute> GetAsync(string routeId);
        Task<bool> TryInsertAsync(ITradingRoute tradingRoute);
        Task UpdateAsync(ITradingRoute tradingRoute);
        Task DeleteAsync(string routeId);
    }
}
