using System;
using System.Collections.Generic;

using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common.WorkingDays;

using Newtonsoft.Json;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSettings : IAuditableObject<AuditDataType>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public List<DateTime> Holidays { get; set; }
        
        public MarketSchedule MarketSchedule { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.MarketSettings;

        public string GetAuditReference() => Id;

        public string ToAuditJson() =>
            JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Converters = new List<JsonConverter> { new WorkingDayConverter() }
                });
    }
}