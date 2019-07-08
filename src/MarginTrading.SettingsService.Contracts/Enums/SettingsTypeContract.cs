// Copyright (c) 2019 Lykke Corp.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.SettingsService.Contracts.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SettingsTypeContract
    {
        AssetPair = 1,
        Asset = 2,
        Market = 3,
        ScheduleSettings = 4,
        ServiceMaintenance = 5,
        TradingCondition = 6,
        TradingInstrument = 7,
        TradingRoute = 8
    }
}