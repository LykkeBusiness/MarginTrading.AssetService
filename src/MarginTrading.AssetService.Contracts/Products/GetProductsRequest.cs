// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Refit;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsRequest
    {
        [Query(CollectionFormat.Multi)]
        public string[] MdsCodes { get; set; }

        [Query(CollectionFormat.Multi)]
        public string[] ProductIds { get; set; }
        
        public bool? IsStarted { get; set; }
    }
}