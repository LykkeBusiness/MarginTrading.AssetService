using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.SettingsService.Client.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchingEngineModeContract
    {
        MarketMaker = 1,
        Stp = 2
    }
}
