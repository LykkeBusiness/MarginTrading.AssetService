// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.Common;
using Lykke.Snow.Common.Percents;

namespace MarginTrading.AssetService.Core.Interfaces
{
    public interface ITradingInstrument
    {
        string TradingConditionId { get; }
        
        string Instrument { get; }
        
        [Obsolete("Use decimal field LeverageIni")]
        int LeverageInit { get; }
        
        [Obsolete("Use decimal field LeverageMnt")]
        int LeverageMaintenance { get; }
        
        Leverage LeverageIni { get; }
        
        Leverage LeverageMnt { get; }
        
        MarginRate MarginRate { get; set; }
        
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
