using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IMarketDayOffService
    {
        Task<Dictionary<string, bool>> MarketsStatus(string[] marketIds);
    }
}