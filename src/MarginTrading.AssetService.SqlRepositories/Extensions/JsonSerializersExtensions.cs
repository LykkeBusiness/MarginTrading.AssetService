using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.SqlRepositories.Extensions
{
    public static class JsonSerializersExtensions
    {
        public static string ToJsonWithStringEnums(this object src, bool ignoreNulls = false)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = ignoreNulls ? NullValueHandling.Ignore : NullValueHandling.Include,
            };
            
            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(src, settings);
        }
    }
}