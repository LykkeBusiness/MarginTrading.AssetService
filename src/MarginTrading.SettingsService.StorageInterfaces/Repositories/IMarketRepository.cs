using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IMarketRepository
    {
        Task<IReadOnlyList<IMarket>> GetAsync();
        Task<IMarket> GetAsync(string marketId);
        Task<bool> TryInsertAsync(IMarket market);
        Task UpdateAsync(IMarket market);
        Task DeleteAsync(string marketId);
    }
}
