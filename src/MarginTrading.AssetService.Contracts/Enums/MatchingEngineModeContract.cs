// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Contracts.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchingEngineModeContract
    {
        MarketMaker = 1,
        Stp = 2
    }
}
