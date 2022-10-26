namespace MarginTrading.AssetService.Contracts.ErrorCodes
{
    public enum ProductsErrorCodesContract
    {
        None,
        AlreadyExists,
        DoesNotExist,
        UnderlyingDoesNotExist,
        CannotCreateCategory,
        CannotCreateProductInNonLeafCategory,
        MarketSettingsDoNotExist,
        CurrencyDoesNotExist,
        TickFormulaDoesNotExist,
        AssetTypeDoesNotExist,
        CanOnlySetFreezeInfoForFrozenProduct,
        CannotFreezeDiscontinuedProduct,
        CannotDeleteStartedProduct,
        CannotChangeStartDateFromPastToFuture,
        LongIsinNotUnique,
        ShortIsinNotUnique,
        CannotChangeDiscontinuedProduct
    }
}