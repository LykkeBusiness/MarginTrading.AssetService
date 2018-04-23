using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.SettingsService.Client.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderDirectionContract
    {
        Buy = 1,
        Sell = 2
    }
}
