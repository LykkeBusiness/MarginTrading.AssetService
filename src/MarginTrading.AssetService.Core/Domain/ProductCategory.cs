using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ProductCategory
    {
        // primary id
        public string Id { get; set; }

        public string LocalizationToken { get; set; }

        public ProductCategory Parent { get; set; }
        public string ParentId { get; set; }

        public bool IsLeaf { get; set; }
        
        public byte[] Timestamp { get; set; }
    }
}