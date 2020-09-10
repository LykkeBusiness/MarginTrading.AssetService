// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IMarketSettingsRepository
    {
        Task<MarketSettings> GetByIdAsync(string id);
        Task<IReadOnlyList<MarketSettings>> GetAllMarketSettingsAsync();
        Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettings model);
        Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettings model);
        Task<Result<MarketSettingsErrorCodes>> DeleteAsync(string id);
    }
}