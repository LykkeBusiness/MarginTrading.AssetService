// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IMarketDayOffService
    {
        Task<Dictionary<string, (DateTime lastTradingDay, bool isTradingEnabled)>> GetMarketsInfo(string[] marketIds);
        
        Task<(DateTime lastTradingDay, bool isTradingEnabled)> GetPlatformInfo();
    }
}