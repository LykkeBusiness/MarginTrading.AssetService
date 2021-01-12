// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Domain
{
    public enum MarketSettingsErrorCodes
    {
        None,
        MarketSettingsDoNotExist,
        IdAlreadyExists,
        NameAlreadyExists,
        InvalidTimezone,
        TradingDayAlreadyStarted,
        InvalidOpenAndCloseHours,
        InvalidDividendsShortValue,
        InvalidDividendsLongValue,
        InvalidDividends871MValue,
        CannotDeleteMarketSettingsAssignedToAnyProduct,
        OpenAndCloseWithAppliedTimezoneMustBeInTheSameDay,
        InconsistentWorkingCalendar
    }
}