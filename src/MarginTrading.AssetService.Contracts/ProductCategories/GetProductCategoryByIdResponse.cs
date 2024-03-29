using Lykke.Contracts.Responses;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class GetProductCategoryByIdResponse : ErrorCodeResponse<ProductCategoriesErrorCodesContract>
    {
        public ProductCategoryContract ProductCategory { get; set; }
    }
}