using System.Collections.Generic;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    public class CheckProductsUnavailableForTradingConditionResponse: ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        public IReadOnlyList<string> UnavailableProductIds { get; set; } = new List<string>();
    }
}