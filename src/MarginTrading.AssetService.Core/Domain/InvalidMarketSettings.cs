// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Domain
{
    public class InvalidMarketSettings : MarketSettings
    {
        public InvalidMarketSettings(MarketSettingsErrorCodes errorCode)
        {
            ErrorCode = errorCode;
        }
        
        public readonly MarketSettingsErrorCodes ErrorCode;
    }
}