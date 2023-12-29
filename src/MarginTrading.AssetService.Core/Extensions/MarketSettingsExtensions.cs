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
            // TODO: probably, would be better to abstract away the time zone conversion
            var timeZone = TZConvert.GetTimeZoneInfo(marketSettings.MarketSchedule.TimeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        /// <summary>
        /// Gets the list of holidays added in the new version of the market settings. 
        /// The comparison is based on the equality of the date portion of 
        /// <see cref="DateTime"/> objects
        /// </summary>
        /// <param name="source">The base version of the market settings</param>
        /// <param name="compareTo">The version to compare to</param>
        /// <returns>List of <see cref="DateTime"/> objects</returns>
        public static IEnumerable<DateTime> AddedHolidays(this MarketSettings source,
            MarketSettings compareTo)
        {
            var dateOnlySourceHolidays = source.Holidays.Select(x => x.Date.Date);
            var dateOnlyNewHolidays = compareTo.Holidays.Select(x => x.Date.Date);
            return dateOnlyNewHolidays.Except(dateOnlySourceHolidays);
        }

        /// <summary>
        /// Gets the list of <see cref="WorkingDay"/> objects in the new version of 
        /// the market settings. The comparison is based on the equality of the 
        /// <see cref="WorkingDay"/> objects.
        /// </summary>
        /// <param name="source">The base version of the market settings</param>
        /// <param name="compareTo">The version to compare to</param>
        /// <returns>List of <see cref="WorkingDay"/> objects</returns>
        public static IEnumerable<WorkingDay> AddedHalfWorkingDays(this MarketSettings source,
            MarketSettings compareTo) => compareTo
                .GetHalfWorkingDays()
                .Except(source.GetHalfWorkingDays());
    }
}