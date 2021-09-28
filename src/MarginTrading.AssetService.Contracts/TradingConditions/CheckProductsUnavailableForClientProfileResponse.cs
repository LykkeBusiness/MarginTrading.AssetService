using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    public class CheckProductsUnavailableForClientProfileResponse
    {
        public IReadOnlyList<string> UnavailableProductIds { get; set; } = new List<string>();
        public string Error { get; set; }
    }
}