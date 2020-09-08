using MarginTrading.AssetService.Contracts.Core;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class AddProductCategoryRequest : UserRequest
    {
        public string Category { get; set; }
    }
}