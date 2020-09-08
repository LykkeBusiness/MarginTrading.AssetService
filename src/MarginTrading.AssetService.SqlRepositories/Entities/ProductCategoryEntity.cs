using System.Collections.Generic;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ProductCategoryEntity
    {
        // primary id
        public string Id { get; set; }

        public string LocalizationToken { get; set; }

        public ProductCategoryEntity Parent { get; set; }
        public string ParentId { get; set; }

        public List<ProductCategoryEntity> Children { get; set; } = new List<ProductCategoryEntity>();
        
        public byte[] Timestamp { get; set; }
    }
}