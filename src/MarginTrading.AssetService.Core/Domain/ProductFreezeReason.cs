using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Core.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductFreezeReason
    {
        Undefined = 0,

        CorporateAction = 1,

        Manual = 2,

        CostAndChargesGeneration = 3
    }
}