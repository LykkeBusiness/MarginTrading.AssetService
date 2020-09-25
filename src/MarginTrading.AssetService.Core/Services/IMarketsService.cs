using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IMarketsService
    {
        Task<IMarket> GetByIdAsync(string id);
        Task<IReadOnlyList<IMarket>> GetAllAsync();
    }
}