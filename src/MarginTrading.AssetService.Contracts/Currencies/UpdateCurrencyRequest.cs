using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class UpdateCurrencyRequest : UserRequest
    {
        public string InterestRateMdsCode  { get; set; }
    }
}