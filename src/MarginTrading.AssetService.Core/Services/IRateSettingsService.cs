// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain.Rates;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IRateSettingsService
    {
        Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRatesAsync(IList<string> assetPairIds = null);
        Task ReplaceOrderExecutionRatesAsync(List<OrderExecutionRate> rates);
        Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync(IList<string> assetPairIds = null);
        Task ReplaceOvernightSwapRatesAsync(List<OvernightSwapRate> rates);
        Task<OnBehalfRate> GetOnBehalfRateAsync();
        Task ReplaceOnBehalfRateAsync(OnBehalfRate rate);
    }
}