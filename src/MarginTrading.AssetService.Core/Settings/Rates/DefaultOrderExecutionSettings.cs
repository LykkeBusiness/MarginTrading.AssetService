// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Settings.Rates
{
    public class DefaultOrderExecutionSettings
    {
        public decimal CommissionCap { get; set; }
        
        public decimal CommissionFloor { get; set; }
        
        public decimal CommissionRate { get; set; }
        
        public string LegalEntity { get; set; }
    }
}