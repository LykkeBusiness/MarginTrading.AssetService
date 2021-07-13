﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    // TODO: Category hierarchy
    public class TradingInstrumentContract
    {
        public string TradingConditionId { get; set; }
        public string Instrument { get; set; }
        public int LeverageInit { get; set; }
        public int LeverageMaintenance { get; set; }
        public decimal SwapLong { get; set; }
        public decimal SwapShort { get; set; }
        public decimal Delta { get; set; }
        public decimal DealMinLimit { get; set; }
        public decimal DealMaxLimit { get; set; }
        public decimal PositionLimit { get; set; }
        public bool ShortPosition { get; set; }
        public decimal LiquidationThreshold { get; set; }
        public decimal OvernightMarginMultiplier { get; set; }
        
        public decimal CommissionRate { get; set; }
        public decimal CommissionMin { get; set; }
        public decimal CommissionMax { get; set; }
        public string CommissionCurrency { get; set; }
        
        public decimal HedgeCost { get; set; }
        
        public decimal Spread { get; set; }
        
        public decimal LeverageIni { get; set; }
        public decimal LeverageMnt { get; set; }
        public decimal MarginRatePercent { get; set; }
    }
}
