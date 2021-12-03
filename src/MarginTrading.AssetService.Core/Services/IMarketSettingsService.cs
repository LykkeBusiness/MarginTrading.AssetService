// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IMarketSettingsService
    {
        Task<MarketSettings> GetByIdAsync(string id);
        Task<IReadOnlyList<MarketSettings>> GetAllMarketSettingsAsync();
        Task<Result<MarketSettingsErrorCodes>> AddAsync(MarketSettingsCreateOrUpdateDto model, string username);
        Task<Result<MarketSettingsErrorCodes>> UpdateAsync(MarketSettingsCreateOrUpdateDto model, string username);
        Task<Result<MarketSettingsErrorCodes>> DeleteAsync(string id, string username);
    }
}