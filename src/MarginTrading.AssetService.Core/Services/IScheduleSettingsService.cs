using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IScheduleSettingsService
    {
        Task<IReadOnlyList<IScheduleSettings>> GetFilteredAsync(string marketId = null);
    }
}