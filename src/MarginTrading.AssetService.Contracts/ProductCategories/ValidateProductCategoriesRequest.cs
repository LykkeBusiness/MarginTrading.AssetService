using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class ValidateProductCategoriesRequest
    {
        public IReadOnlyList<ProductAndCategoryPairContract> Pairs { get; set; }
    }
}