// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
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
