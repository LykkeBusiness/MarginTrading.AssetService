using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Core.Constants;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSettings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public List<DateTime> Holidays { get; set; }
        
        public MarketSchedule MarketSchedule { get; set; }

        public static MarketSettings GetMarketSettingsWithDefaults(MarketSettingsCreateOrUpdateDto model)
        {
            var open = model.Open.Any() ? model.Open : new[] { MarketSettingsConstants.DefaultOpen };
            var close = model.Close.Any() ? model.Close : new[] { MarketSettingsConstants.DefaultClose };
            var timeZone = !string.IsNullOrEmpty(model.Timezone) ? model.Timezone: MarketSettingsConstants.DefaultTimeZone;

            return new MarketSettings
            {
                Id = model.Id,
                Name = model.Name,
                Dividends871M = model.Dividends871M,
                DividendsLong = model.DividendsLong,
                DividendsShort = model.DividendsShort,
                Holidays = model.Holidays,
                MarketSchedule =
                    new MarketSchedule(open, close, timeZone, model.HalfWorkingDays)
            };
        }
    }
}