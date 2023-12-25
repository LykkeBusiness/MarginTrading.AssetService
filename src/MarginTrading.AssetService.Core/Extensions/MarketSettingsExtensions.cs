// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Lykke.Snow.Common.WorkingDays;

using MarginTrading.AssetService.Core.Domain;

using TimeZoneConverter;

namespace MarginTrading.AssetService.Core.Extensions
{
    public static class MarketSettingsExtensions
    {
        public static DateTime ConvertToMarketTimeZone(this MarketSettings marketSettings, DateTime dateTime)
        {
            var timeZone = TZConvert.GetTimeZoneInfo(marketSettings.MarketSchedule.TimeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        public static IEnumerable<DateTime> GetAddedHolidays(this MarketSettings existingVersion,
            MarketSettings newVersion)
        {
            var dateOnlyExistingHolidays = existingVersion.Holidays.Select(x => x.Date.Date);
            var dateOnlyNewHolidays = newVersion.Holidays.Select(x => x.Date.Date);
            return dateOnlyNewHolidays.Except(dateOnlyExistingHolidays);
        }

        public static IEnumerable<WorkingDay> GetAddedHalfWorkingDays(this MarketSettings existingVersion,
            MarketSettings newVersion)
        {
            return newVersion
                .GetHalfWorkingDays()
                .Except(existingVersion.GetHalfWorkingDays());
        }
    }
}