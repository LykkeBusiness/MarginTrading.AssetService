// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain.Rates;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IRateSettingsService
    {
        Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync(string clientProfileId, IList<string> assetPairIds = null);
    }
}