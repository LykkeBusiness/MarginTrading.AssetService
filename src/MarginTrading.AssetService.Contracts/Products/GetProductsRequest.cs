// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using JetBrains.Annotations;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsRequest
    {
        [CanBeNull] public string MdsCodeFilter { get; set; }
        
        public string[] MdsCodes { get; set; } = Array.Empty<string>();

        public string[] ProductIds { get; set; } = Array.Empty<string>();
        
        public bool? IsStarted { get; set; }

        public bool? IsDiscontinued { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}