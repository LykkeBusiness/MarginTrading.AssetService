namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class CurrencyEntity
    {
        public string Id { get; set; }
        public string InterestRateMdsCode  { get; set; }
        public int Accuracy { get; set; }
        
        public byte[] Timestamp { get; set; }
    }
}