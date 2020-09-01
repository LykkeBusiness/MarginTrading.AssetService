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
    }
}