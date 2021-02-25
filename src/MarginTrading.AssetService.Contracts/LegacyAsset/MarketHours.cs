// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class MarketHours
    {
        [JsonProperty("close")]
        public TimeSpan[] Close { get; set; }

        [JsonProperty("day")]
        public DateTime Day { get; set; }

        [JsonProperty("open")]
        public TimeSpan[] Open { get; set; }
    }
}