// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class DividendsFactor
    {
        [JsonProperty("shortPercent")]
        public decimal ShortPercent { get; set; }

        [JsonProperty("percent")]
        public decimal Percent { get; set; }

        [JsonProperty("us871Percent")]
        public decimal Us871Percent { get; set; }
    }
}