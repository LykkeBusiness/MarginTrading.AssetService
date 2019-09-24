﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface ITradingInstrument
    {
        string TradingConditionId { get; }
        string Instrument { get; }
        int LeverageInit { get; }
        int LeverageMaintenance { get; }
        decimal SwapLong { get; }
        decimal SwapShort { get; }
        decimal Delta { get; }
        decimal DealMinLimit { get; }
        decimal DealMaxLimit { get; }
        decimal PositionLimit { get; }
        bool ShortPosition { get; } 
        decimal LiquidationThreshold { get; }
        decimal OvernightMarginMultiplier { get; }
        
        decimal CommissionRate { get; }
        decimal CommissionMin { get; }
        decimal CommissionMax { get; }
        string CommissionCurrency { get; }
        decimal HedgeCost { get; }
        decimal Spread { get; }
    }
}
