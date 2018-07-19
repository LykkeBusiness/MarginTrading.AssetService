using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface ITradingRoutesRepository
    {
        Task<IReadOnlyList<ITradingRoute>> GetAsync();
        Task<PaginatedResponse<ITradingRoute>> GetByPagesAsync(int? skip = null, int? take = null);
        Task<ITradingRoute> GetAsync(string routeId);
        Task<bool> TryInsertAsync(ITradingRoute tradingRoute);
        Task UpdateAsync(ITradingRoute tradingRoute);
        Task DeleteAsync(string routeId);
    }
}
