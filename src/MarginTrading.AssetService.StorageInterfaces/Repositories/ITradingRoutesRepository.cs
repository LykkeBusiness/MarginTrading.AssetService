// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
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
