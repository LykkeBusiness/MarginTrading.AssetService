// Copyright (c) 2019 Lykke Corp.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.SettingsService.Contracts.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderDirectionContract
    {
        Buy = 1,
        Sell = 2
    }
}
