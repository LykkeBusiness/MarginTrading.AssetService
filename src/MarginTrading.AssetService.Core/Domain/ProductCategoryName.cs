namespace MarginTrading.AssetService.Core.Domain
{
    public class ProductCategoryName
    {
        public string NormalizedName { get; }

        public ProductCategoryName(string category)
        {
            // todo: tests
            NormalizedName = category
                .ToLower()
                .Replace(' ', '_')
                .Replace('/', '.');
        }
    }
}