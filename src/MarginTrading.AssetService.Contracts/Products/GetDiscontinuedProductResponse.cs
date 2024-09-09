using System;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetDiscontinuedProductResponse
    {
        public DiscontinuedProduct[] DiscontinuedProducts { get; set; } = Array.Empty<DiscontinuedProduct>();
    }
}