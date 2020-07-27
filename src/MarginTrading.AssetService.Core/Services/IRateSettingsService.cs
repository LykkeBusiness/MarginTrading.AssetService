// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain.Rates;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IRateSettingsService
    {
        Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRates(IList<string> assetPairIds = null);
        Task ReplaceOrderExecutionRates(List<OrderExecutionRate> rates);
        Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRates();
        Task ReplaceOvernightSwapRates(List<OvernightSwapRate> rates);
        Task<OnBehalfRate> GetOnBehalfRate();
        Task ReplaceOnBehalfRate(OnBehalfRate rate);
    }
}