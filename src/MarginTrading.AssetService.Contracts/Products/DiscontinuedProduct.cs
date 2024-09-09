using System;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class DiscontinuedProduct
    {
        public DateOnly? ActualDiscontinuedDate { get; set; }
        public string ProductId { get; set; }
    }
}