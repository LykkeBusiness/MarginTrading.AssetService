using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class ValidateProductCategoriesResponse
    {
        public IReadOnlyList<string> ErrorMessages { get; set; }
    }
}