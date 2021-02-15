// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class ExecutionFeeParameter
    {
        [JsonProperty("assetType")]
        public string AssetType { get; set; }

        [JsonProperty("commissionCap")]
        public decimal CommissionCap { get; set; }

        [JsonProperty("commissionFloor")]
        public decimal CommissionFloor { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("ratePercent")]
        public decimal RatePercent { get; set; }
    }
}