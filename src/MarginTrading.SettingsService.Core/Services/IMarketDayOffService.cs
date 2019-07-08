// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IMarketDayOffService
    {
        Task<Dictionary<string, bool>> MarketsStatus(string[] marketIds);
        
        Task<(DateTime, bool)> GetPlatformInfo();
    }
}