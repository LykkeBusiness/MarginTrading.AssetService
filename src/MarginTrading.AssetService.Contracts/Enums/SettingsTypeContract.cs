// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Contracts.Enums
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