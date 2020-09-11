namespace MarginTrading.AssetService.Core.Domain
{
    public class Currency
    {
        public string Id { get; set; }
        public string InterestRateMdsCode  { get; set; }

        public int Accuracy { get; set; } = 2;
        
        public byte[] Timestamp { get; set; }
    }
}