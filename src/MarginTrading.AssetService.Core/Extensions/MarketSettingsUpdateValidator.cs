// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Extensions
{
    /// <summary>
    /// Checks if the market settings can be updated. Takes current version
    /// of the market settings and the supposed new version as parameters.
    /// </summary>
    public class MarketSettingsUpdateValidator
    {
        private readonly MarketSettings _versionInUse;
        private readonly MarketSettings _futureVersion;

        public MarketSettingsUpdateValidator(MarketSettings existingVersion, MarketSettings futureVersion)
        {
            _versionInUse = existingVersion ?? throw new ArgumentNullException(nameof(existingVersion));
            _futureVersion = futureVersion ?? throw new ArgumentNullException(nameof(futureVersion));
        }

        /// <summary>
        /// Checks if update is valid at the given date when the market is open.
        /// </summary>
        /// <param name="date">The date and time update should be applied</param>
        /// <returns></returns>
        public bool ValidAt(DateTime date)
        {
            var dateAtMarketTimeZone = _versionInUse.ConvertToMarketTimeZone(date);

            if (AddedHolidaysIncludeDate(dateAtMarketTimeZone) ||
                AddedHalfWorkingDaysIncludeDate(dateAtMarketTimeZone))
            {
                var isMarketAlreadyOpen = dateAtMarketTimeZone.TimeOfDay >= _versionInUse.GetMarketOpenTime();
                if (isMarketAlreadyOpen)
                    return false;
            }

            return true;
        }
        
        private bool AddedHolidaysIncludeDate(DateTime day)
        {
            var addedHolidays = _versionInUse.GetAddedHolidays(_futureVersion);
            var dayOnly = day.Date;
            return addedHolidays.Contains(dayOnly);
        }

        private bool AddedHalfWorkingDaysIncludeDate(DateTime day)
        {
            var addedHalfWorkingDays = _versionInUse.GetAddedHalfWorkingDays(_futureVersion);
            var dayOnly = day.Date;
            return addedHalfWorkingDays.Any(d => d.SameCalendarDay(dayOnly));
        }
    }
}