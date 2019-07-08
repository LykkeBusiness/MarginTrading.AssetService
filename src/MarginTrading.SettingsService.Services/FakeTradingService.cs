// Copyright (c) 2019 Lykke Corp.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Services;

namespace MarginTrading.SettingsService.Services
{
    public class FakeTradingService : ITradingService
    {
#pragma warning disable 1998
        public async Task<IReadOnlyList<string>> CheckActiveByTradingCondition(string tradingConditionId)
        {
            return new List<string>();
        }
    }
}