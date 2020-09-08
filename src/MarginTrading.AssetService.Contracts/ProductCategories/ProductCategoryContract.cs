namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class ProductCategoryContract
    {
        // primary id
        public string Id { get; set; }

        public string LocalizationToken { get; set; }

        public string ParentId { get; set; }

        public bool IsLeaf { get; set; }
    }
}