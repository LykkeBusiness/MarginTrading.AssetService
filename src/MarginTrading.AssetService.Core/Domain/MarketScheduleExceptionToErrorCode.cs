// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Lykke.Snow.Common.Exceptions;

namespace MarginTrading.AssetService.Core.Domain
{
    internal static class MarketScheduleExceptionToErrorCode
    {
        internal static readonly Dictionary<Type, MarketSettingsErrorCodes> Map =
            new Dictionary<Type, MarketSettingsErrorCodes>
            {
                {
                    typeof(InvalidOpenAndCloseHoursException), 
                    MarketSettingsErrorCodes.InvalidOpenAndCloseHours
                },
                {
                    typeof(WinterOpenAndCloseWithAppliedTimezoneMustBeInTheSameDayException),
                    MarketSettingsErrorCodes.WinterOpenAndCloseWithAppliedTimezoneMustBeInTheSameDay
                },
                {
                    typeof(SummerOpenAndCloseWithAppliedTimezoneMustBeInTheSameDayException),
                    MarketSettingsErrorCodes.SummerOpenAndCloseWithAppliedTimezoneMustBeInTheSameDay
                },
                {
                    typeof(InvalidTimeZoneException), 
                    MarketSettingsErrorCodes.InvalidTimezone
                },
                {
                    typeof(InconsistentWorkingCalendarException),
                    MarketSettingsErrorCodes.InconsistentWorkingCalendar
                },
                {
                    typeof(InvalidWorkingDayStringException), 
                    MarketSettingsErrorCodes.InvalidHalfWorkingDayString
                }
            };
    }
}