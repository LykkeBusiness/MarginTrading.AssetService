using Lykke.Contracts.Responses;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductByIdResponse : ErrorCodeResponse<ProductsErrorCodesContract>
    {
        public ProductContract Product { get; set; }
    }
}