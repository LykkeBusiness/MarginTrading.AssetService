using MessagePack;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    [MessagePackObject]
    public class ProductCategoryContract
    {
        // primary id
        [Key(0)]
        public string Id { get; set; }
        [Key(1)]
        public string LocalizationToken { get; set; }
        [Key(2)]
        public string ParentId { get; set; }
        [Key(3)]
        public bool IsLeaf { get; set; }
    }
}