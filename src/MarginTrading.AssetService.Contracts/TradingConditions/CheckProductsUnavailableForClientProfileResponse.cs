using System.Collections.Generic;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    public class CheckProductsUnavailableForClientProfileResponse: ErrorCodeResponse<ClientProfilesErrorCodesContract>
    {
        public IReadOnlyList<string> UnavailableProductIds { get; set; } = new List<string>();
    }
}