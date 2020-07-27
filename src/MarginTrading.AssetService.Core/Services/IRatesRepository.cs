// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain.Rates;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IRatesRepository
    {
        Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRatesAsync();
        Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync();
        Task<OnBehalfRate> GetOnBehalfRateAsync();

        Task MergeOrderExecutionRatesAsync(IReadOnlyList<OrderExecutionRate> rates);
        Task MergeOvernightSwapRatesAsync(IReadOnlyList<OvernightSwapRate> rates);
        Task ReplaceOnBehalfRateAsync(OnBehalfRate rate);
    }
}