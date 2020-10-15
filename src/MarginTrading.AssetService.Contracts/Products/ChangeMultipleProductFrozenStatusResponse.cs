using System.Collections.Generic;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class ChangeMultipleProductFrozenStatusResponse : ErrorCodeResponse<ProductsErrorCodesContract>
    {
        public IDictionary<string, ChangeProductFrozenStatusResponse> Results { get; set; }
    }
}