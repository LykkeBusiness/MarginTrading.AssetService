// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class HolidayEntity
    {
        public string MarketSettingsId { get; set; }
        public DateTime Date { get; set; }
    }
}