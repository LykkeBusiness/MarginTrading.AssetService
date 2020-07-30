// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
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
