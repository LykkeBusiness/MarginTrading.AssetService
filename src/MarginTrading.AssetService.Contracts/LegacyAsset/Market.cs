// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class Market
    {
        [JsonProperty("calendar")]
        public Calendar Calendar { get; set; }

        [JsonProperty("marketHours")]
        public MarketHours MarketHours { get; set; }

        [JsonProperty("marketId")]
        public string MarketId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dividendsFactor")]
        public DividendsFactor DividendsFactor { get; set; }
    }
}