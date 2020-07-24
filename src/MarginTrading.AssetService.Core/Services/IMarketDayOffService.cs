// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IMarketDayOffService
    {
        Task<Dictionary<string, TradingDayInfo>> GetMarketsInfo(string[] marketIds, DateTime? dateTime);
        
        Task<TradingDayInfo> GetPlatformInfo(DateTime? dateTime);
    }
}