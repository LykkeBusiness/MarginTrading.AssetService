using System;
using Lykke.Snow.Common.WorkingDays;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketScheduleConverter : JsonConverter<MarketSchedule>
    {
        public override bool CanWrite
        {
            get => false;
        }

        public override void WriteJson(JsonWriter writer, MarketSchedule value, JsonSerializer serializer)
        {
            throw new NotImplementedException($"{nameof(MarketScheduleConverter)} is not supposed to be used for writing json value.");
        }

        public override MarketSchedule ReadJson(JsonReader reader,
            Type objectType,
            MarketSchedule existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);

            var open = o.GetValue(nameof(MarketSchedule.Open))?.ToObject<TimeSpan[]>();
            var close = o.GetValue(nameof(MarketSchedule.Close))?.ToObject<TimeSpan[]>();
            var timezoneId = o.GetValue(nameof(MarketSchedule.TimeZoneId))?.ToString();
            var halfWorkingDays = o.GetValue(nameof(MarketSchedule.HalfWorkingDays))?.ToObject<string[]>();

            return new MarketSchedule(open, close, timezoneId ?? TimeZoneInfo.Utc.Id, halfWorkingDays);
        }
    }
}