// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Contracts.AssetPair
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FreezeReasonContract
    {
        Undefined = 0,
        
        CorporateAction = 1,
        
        Manual = 2,
        
        CostAndChargesGeneration = 3
    }
}