using System.Collections.Generic;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class DeleteProductBatchRequest : UserRequest
    {
        public IReadOnlyList<string> ProductIds { get; set; }
    }
}