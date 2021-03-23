using System;
using Refit;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsCountRequest
    {
        [Query(CollectionFormat.Multi)]
        public string[] MdsCodes { get; set; } = Array.Empty<string>();

        [Query(CollectionFormat.Multi)]
        public string[] ProductIds { get; set; } = Array.Empty<string>();
    }
}