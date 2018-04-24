using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MarginTrading.SettingsService.Services
{
    public class FakeTradingService : ITradingService
    {
        public async Task<IReadOnlyList<string>> CheckActiveByTradingCondition(string tradingConditionId)
        {
            return new List<string>();
        }
    }
}