// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Lykke.Snow.Common.WorkingDays;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class Calendar
    {
        [JsonProperty("holidays")]
        public List<DateTime> Holidays { get; set; }

        [JsonProperty("halfWorkingDays")]
        public List<WorkingDay> HalfWorkingDays { get; set; }
    }
}