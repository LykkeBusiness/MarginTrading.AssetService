namespace MarginTrading.AssetService.Core.Domain
{
    public enum ProductsErrorCodes
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
        CannotChangeDiscontinuedProduct,
        CannotUpdateContractSizeForStarted,
    }
}