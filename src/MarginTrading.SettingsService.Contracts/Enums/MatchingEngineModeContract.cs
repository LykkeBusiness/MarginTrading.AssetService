// Copyright (c) 2019 Lykke Corp.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.SettingsService.Contracts.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchingEngineModeContract
    {
        MarketMaker = 1,
        Stp = 2
    }
}
