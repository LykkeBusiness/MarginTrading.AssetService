using System;
// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Constants
{
    public static class MarketSettingsConstants
    {
        public static readonly string DefaultTimeZone = TimeZoneInfo.Utc.Id;
        public static readonly TimeSpan DefaultOpen = new TimeSpan(8, 0, 0);
        public static readonly TimeSpan DefaultClose = new TimeSpan(22, 0, 0);
    }
}