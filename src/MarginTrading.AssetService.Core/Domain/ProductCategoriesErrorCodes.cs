namespace MarginTrading.AssetService.Core.Domain
{
    public enum ProductCategoriesErrorCodes
    {
        None,
        AlreadyExists,
        DoesNotExist,
        CannotDeleteNonLeafCategory,
        ParentHasAttachedProducts,
        CannotDeleteCategoryWithAttachedProducts,
        CategoryStringIsNotValid,
    }
}