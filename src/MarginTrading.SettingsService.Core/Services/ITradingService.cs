// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface ITradingService
    {
        /// <summary>
        /// Check active orders by trading condition Id and return result grouped by instrument in format: $"{group.Key}({group.Count()}
        /// </summary>
        /// <param name="tradingConditionId"></param>
        /// <returns></returns>
        Task<IReadOnlyList<string>> CheckActiveByTradingCondition(string tradingConditionId);
    }
}