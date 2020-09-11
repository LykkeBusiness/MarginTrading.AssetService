using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class GetProductCategoriesResponse
    {
        public IReadOnlyList<ProductCategoryContract> ProductCategories { get; set; }
    }
}