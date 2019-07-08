// Copyright (c) 2019 Lykke Corp.

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