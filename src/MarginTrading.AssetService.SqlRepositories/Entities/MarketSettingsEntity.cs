// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class MarketSettingsEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public virtual List<HolidayEntity> Holidays { get; set; }
        
        public MarketScheduleEntity MarketSchedule { get; set; }

        public static MarketSettingsEntity Create(MarketSettings model)
        {
            return new MarketSettingsEntity
            {
                Id = model.Id,
                Name = model.Name,
                DividendsLong = model.DividendsLong,
                DividendsShort = model.DividendsShort,
                Dividends871M = model.Dividends871M,
                NormalizedName = model.Name.ToLower(),
                Holidays = model.Holidays.Distinct().Select(date => new HolidayEntity
                {
                    Date = date.Date,
                    MarketSettingsId = model.Id
                }).ToList(),
                MarketSchedule = new MarketScheduleEntity {Schedule = model.MarketSchedule}
            };
        }

        public void MapFromDomain(MarketSettings model)
        {
            Name = model.Name;
            DividendsLong = model.DividendsLong;
            DividendsShort = model.DividendsShort;
            Dividends871M = model.Dividends871M;
            NormalizedName = model.Name.ToLower();
            Holidays = model.Holidays.Distinct().Select(date => new HolidayEntity
            {
                Date = date.Date,
                MarketSettingsId = model.Id
            }).ToList();
            MarketSchedule = new MarketScheduleEntity {Schedule = model.MarketSchedule};
        }
    }
}