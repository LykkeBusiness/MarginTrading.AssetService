namespace MarginTrading.AssetService.Core.Caches
{
    public class UnderlyingCategoryCacheModel
    {
        public string Id { get; set; }
        public string FinancingFeesFormula { get; set; }

        public static UnderlyingCategoryCacheModel Create(string id, string financingFeesFormula)
            => new UnderlyingCategoryCacheModel
            {
                Id = id,
                FinancingFeesFormula = financingFeesFormula,
            };
    }
}