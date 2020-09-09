namespace MarginTrading.AssetService.Contracts.ErrorCodes
{
    public enum ProductCategoriesErrorCodesContract
    {
        None,
        AlreadyExists,
        DoesNotExist,
        CannotDeleteNonLeafCategory,
        ParentHasAttachedProducts,
        CannotDeleteCategoryWithAttachedProducts,
    }
}