namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class AssetTypeEntity
    {
        public string Id { get; set; }
        public string RegulatoryTypeId { get; set; }
        public string UnderlyingCategoryId { get; set; }
        public bool ExcludeSpreadFromProductCosts { get; set; }
    }
}