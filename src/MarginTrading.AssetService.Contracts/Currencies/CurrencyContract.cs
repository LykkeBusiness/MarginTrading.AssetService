using MessagePack;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    [MessagePackObject]
    public class CurrencyContract
    {
        [Key(0)]
        public string Id { get; set; }
        [Key(1)]
        public string InterestRateMdsCode  { get; set; }
        [Key(2)]
        public int Accuracy { get; set; }
    }
}