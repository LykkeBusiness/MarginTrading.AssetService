// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Contracts.ErrorCodes
{
    public enum MarketSettingsErrorCodesContract
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
    }
}