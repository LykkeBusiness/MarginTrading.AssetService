namespace MarginTrading.AssetService.Core.Domain
{
    public enum ProductsErrorCodes
    {
        None,
        AlreadyExists,
        DoesNotExist,
        UnderlyingDoesNotExist,
        MarketSettingsDoNotExist,
    }
}