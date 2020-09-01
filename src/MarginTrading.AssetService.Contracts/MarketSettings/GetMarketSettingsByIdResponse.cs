using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    /// <summary>
    /// Response model to get market settings by ids
    /// </summary>
    public class GetMarketSettingsByIdResponse : ErrorCodeResponse<MarketSettingsErrorCodesContract>
    {
        /// <summary>
        /// MarketSettings
        /// </summary>
        public MarketSettingsContract MarketSettings { get; set; }
    }
}