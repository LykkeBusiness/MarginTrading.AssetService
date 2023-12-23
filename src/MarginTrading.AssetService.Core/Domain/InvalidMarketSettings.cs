// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class InvalidMarketSettings : MarketSettings
    {
        public InvalidMarketSettings(MarketSettingsErrorCodes errorCode)
        {
            if (errorCode == MarketSettingsErrorCodes.None)
                throw new ArgumentException("Error code must be specified, None is not allowed", nameof(errorCode));
            
            ErrorCode = errorCode;
        }
        
        public readonly MarketSettingsErrorCodes ErrorCode;
    }
}