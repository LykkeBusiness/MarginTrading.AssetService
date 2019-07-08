// Copyright (c) 2019 Lykke Corp.

using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Core.Settings
{
    public class DefaultTradingInstrumentSettings
    {
        [Optional]
        public int LeverageInit { get; set; }

        [Optional]
        public int LeverageMaintenance { get; set; }

        [Optional]
        public decimal SwapLong { get; set; }

        [Optional]
        public decimal SwapShort { get; set; }

        [Optional]
        public decimal Delta { get; set; }

        [Optional]
        public decimal DealMinLimit { get; set; }

        [Optional]
        public decimal DealMaxLimit { get; set; }

        [Optional]
        public decimal PositionLimit { get; set; }
        
        [Optional]
        public decimal LiquidationThreshold { get; set; }

        [Optional] 
        public decimal OvernightMarginMultiplier { get; set; } = 1;

        [Optional]
        public decimal CommissionRate { get; set; }

        [Optional]
        public decimal CommissionMin { get; set; }

        [Optional]
        public decimal CommissionMax { get; set; }

        [Optional]
        public string CommissionCurrency { get; set; }
    }
}