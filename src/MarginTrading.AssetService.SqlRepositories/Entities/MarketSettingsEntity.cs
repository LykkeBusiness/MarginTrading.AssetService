// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class MarketSettingsEntity
    {
        public string Id { get; set; }

        public string MICCode { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public decimal DividendsLong { get; set; }

        public decimal DividendsShort { get; set; }

        public decimal Dividends871M { get; set; }

        public TimeSpan Open { get; set; }

        public TimeSpan Close { get; set; }

        public string Timezone { get; set; }

        public virtual List<HolidayEntity> Holidays { get; set; }

        public static MarketSettingsEntity Create(MarketSettings model)
        {
            return new MarketSettingsEntity
            {
                Id = model.Id,
                Name = model.Name,
                DividendsLong = model.DividendsLong,
                DividendsShort = model.DividendsShort,
                Dividends871M = model.Dividends871M,
                MICCode = model.MICCode,
                Close = model.Close,
                Open = model.Open,
                Timezone = model.Timezone,
                NormalizedName = model.Name.ToLower(),
                Holidays = model.Holidays.Distinct().Select(date => new HolidayEntity
                {
                    Date = date.Date,
                    MarketSettingsId = model.Id
                }).ToList(),
            };
        }

        public void MapFromDomain(MarketSettings model)
        {
            Name = model.Name;
            DividendsLong = model.DividendsLong;
            DividendsShort = model.DividendsShort;
            Dividends871M = model.Dividends871M;
            MICCode = model.MICCode;
            Close = model.Close;
            Open = model.Open;
            Timezone = model.Timezone;
            NormalizedName = model.Name.ToLower();
            Holidays = model.Holidays.Distinct().Select(date => new HolidayEntity
            {
                Date = date.Date,
                MarketSettingsId = model.Id
            }).ToList();
        }
    }
}