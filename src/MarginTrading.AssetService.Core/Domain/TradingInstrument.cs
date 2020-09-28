// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class TradingInstrument : ITradingInstrument
    {
        public TradingInstrument(string tradingConditionId, string instrument, int leverageInit,
            int leverageMaintenance, decimal swapLong, decimal swapShort, decimal delta, decimal dealMinLimit,
            decimal dealMaxLimit, decimal positionLimit, bool shortPosition, decimal liquidationThreshold,
            decimal overnightMarginMultiplier, decimal commissionRate, decimal commissionMin, decimal commissionMax,
            string commissionCurrency, decimal hedgeCost, decimal spread)
        {
            TradingConditionId = tradingConditionId;
            Instrument = instrument;
            LeverageInit = leverageInit;
            LeverageMaintenance = leverageMaintenance;
            SwapLong = swapLong;
            SwapShort = swapShort;
            Delta = delta;
            DealMinLimit = dealMinLimit;
            DealMaxLimit = dealMaxLimit;
            PositionLimit = positionLimit;
            ShortPosition = shortPosition;
            LiquidationThreshold = liquidationThreshold;
            OvernightMarginMultiplier = overnightMarginMultiplier;
            CommissionRate = commissionRate;
            CommissionMin = commissionMin;
            CommissionMax = commissionMax;
            CommissionCurrency = commissionCurrency;
            HedgeCost = hedgeCost;
            Spread = spread;
        }

        public string TradingConditionId { get; }
        public string Instrument { get; }
        public int LeverageInit { get; }
        public int LeverageMaintenance { get; }
        public decimal SwapLong { get; }
        public decimal SwapShort { get; }
        public decimal Delta { get; }
        public decimal DealMinLimit { get; }
        public decimal DealMaxLimit { get; }
        public decimal PositionLimit { get; }
        public bool ShortPosition { get; }
        public decimal LiquidationThreshold { get; }
        public decimal OvernightMarginMultiplier { get; }
        public decimal CommissionRate { get; }
        public decimal CommissionMin { get; }
        public decimal CommissionMax { get; }
        public string CommissionCurrency { get; }
        public decimal HedgeCost { get; }
        public decimal Spread { get; }

        public static TradingInstrument CreateFromProduct(Product product, string profileId, decimal marginRate,
            decimal hedgeCost, decimal spread)
        {
            return new TradingInstrument(
                tradingConditionId: profileId,
                instrument: product.ProductId,
                leverageInit: (int)(1 / marginRate / 100),
                leverageMaintenance: (int)(1 / marginRate / 100),
                swapLong: TradingInstrumentsConstants.SwapLong,
                swapShort: TradingInstrumentsConstants.SwapShort,
                delta: TradingInstrumentsConstants.Delta,
                dealMinLimit:product.MinOrderSize,
                dealMaxLimit:product.MaxOrderSize,
                positionLimit:product.MaxPositionSize,
                shortPosition:product.ShortPosition,
                liquidationThreshold: TradingInstrumentsConstants.LiquidationThreshold,
                overnightMarginMultiplier:product.OvernightMarginMultiplier,
                commissionRate:TradingInstrumentsConstants.CommissionRate,
                commissionMin:TradingInstrumentsConstants.CommissionMin,
                commissionMax:TradingInstrumentsConstants.CommissionMax,
                commissionCurrency:TradingInstrumentsConstants.CommissionCurrency,
                hedgeCost:hedgeCost,
                spread:spread
            );
        }
    }
}