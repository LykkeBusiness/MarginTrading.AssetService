using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    public class CheckProductsUnavailableForTradingConditionResponse
    {
        public IReadOnlyList<string> UnavailableProductIds { get; set; }
    }
}