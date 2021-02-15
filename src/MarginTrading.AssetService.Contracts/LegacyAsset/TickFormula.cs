// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class TickFormula
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("params")]
        public TickFormulaParameters TickFormulaParameters { get; set; }
    }
}