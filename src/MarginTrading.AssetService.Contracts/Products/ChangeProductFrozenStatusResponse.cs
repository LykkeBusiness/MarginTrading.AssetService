using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class ChangeProductFrozenStatusResponse : ErrorCodeResponse<ProductsErrorCodesContract>
    {
        public ProductContract Product { get; set; }
    }
}