// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class TickFormulaParameters
    {
        [JsonProperty("ladders")]
        public List<decimal> Ladders { get; set; }

        [JsonProperty("values")]
        public List<decimal> Values { get; set; }
    }
}