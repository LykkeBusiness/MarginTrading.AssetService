namespace MarginTrading.AssetService.Core.Domain
{
    public enum CurrenciesErrorCodes
    {
        None,
        AlreadyExists,
        DoesNotExist,
        CannotDeleteCurrencyWithAttachedProducts,
    }
}