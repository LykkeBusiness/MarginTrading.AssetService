// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsRequest
    {
        public string[] MdsCodes { get; set; } = Array.Empty<string>();

        public string[] ProductIds { get; set; } = Array.Empty<string>();
        
        public bool? IsStarted { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}