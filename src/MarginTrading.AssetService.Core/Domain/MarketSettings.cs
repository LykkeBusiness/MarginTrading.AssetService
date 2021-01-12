using System;
using System.Collections.Generic;

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
            return new MarketSettings
            {
                Id = model.Id,
                Name = model.Name,
                Dividends871M = model.Dividends871M,
                DividendsLong = model.DividendsLong,
                DividendsShort = model.DividendsShort,
                Holidays = model.Holidays,
                MarketSchedule =
                    new MarketSchedule(model.Open, model.Close, model.Timezone, model.HalfWorkingDays)
            };
        }
    }
}