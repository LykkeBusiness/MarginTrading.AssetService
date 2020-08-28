using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsResponse
    {
        public IReadOnlyList<ProductContract> Products { get; set; }
    }
}