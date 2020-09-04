using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class AddCurrencyRequest : UserRequest
    {
        public string Id { get; set; }
        public string InterestRateMdsCode  { get; set; }
    }
}