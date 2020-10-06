using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class UpdateProductBatchRequest
    {
        public IReadOnlyDictionary<string, UpdateProductRequest> Requests { get; set; }
    }
}