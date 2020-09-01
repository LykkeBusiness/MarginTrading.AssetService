// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSettingsCreateOrUpdateDto
    {
        public string Id { get; set; }

        public string MICCode { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public decimal DividendsLong { get; set; }

        public decimal DividendsShort { get; set; }

        public decimal Dividends871M { get; set; }

        public TimeSpan? Open { get; set; }

        public TimeSpan? Close { get; set; }

        public string Timezone { get; set; }

        public List<DateTime> Holidays { get; set; }
    }
}