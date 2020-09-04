namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class CurrencyContract
    {
        public string Id { get; set; }
        public string InterestRateMdsCode  { get; set; }
        public int Accuracy { get; set; }
    }
}