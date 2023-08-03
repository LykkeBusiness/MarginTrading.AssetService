// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.Common;
using Lykke.Snow.Common.Percents;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class TradingInstrument : ITradingInstrument
    {
        public TradingInstrument(string tradingConditionId,
            string instrument,
            decimal swapLong,
            decimal swapShort,
            decimal delta,
            decimal dealMinLimit,
            decimal dealMaxLimit,
            decimal positionLimit,
            bool shortPosition,
            decimal liquidationThreshold,
            decimal overnightMarginMultiplier,
            decimal commissionRate,
            decimal commissionMin,
            decimal commissionMax,
            string commissionCurrency,
            decimal hedgeCost,
            decimal spread,
            Leverage initLeverage,
            Leverage maintenanceLeverage,
            MarginRate marginRate,
            decimal? maxPositionNotional)
        {
            TradingConditionId = tradingConditionId;
            Instrument = instrument;
            LeverageInit = (int) initLeverage;
            LeverageMaintenance = (int) maintenanceLeverage;
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
            InitLeverage = initLeverage;
            MaintenanceLeverage = maintenanceLeverage;
            MarginRate = marginRate;
            MaxPositionNotional = maxPositionNotional;
        }

        public string TradingConditionId { get; }
        public string Instrument { get; }
        public int LeverageInit { get; }
        public int LeverageMaintenance { get; }
        public Leverage InitLeverage { get; }
        public Leverage MaintenanceLeverage { get; }
        public MarginRate MarginRate { get; set; }

        [Obsolete]
        public decimal SwapLong { get; }
        [Obsolete]
        public decimal SwapShort { get; }
        [Obsolete]
        public decimal Delta { get; }
        public decimal DealMinLimit { get; }
        public decimal DealMaxLimit { get; }
        public decimal PositionLimit { get; }
        public bool ShortPosition { get; }
        [Obsolete]
        public decimal LiquidationThreshold { get; }
        public decimal OvernightMarginMultiplier { get; }
        [Obsolete]
        public decimal CommissionRate { get; }
        [Obsolete]
        public decimal CommissionMin { get; }
        [Obsolete]
        public decimal CommissionMax { get; }
        [Obsolete]
        public string CommissionCurrency { get; }
        public decimal HedgeCost { get; }
        public decimal Spread { get; }

        public decimal? MaxPositionNotional { get; }

        public static TradingInstrument CreateFromProduct(Product product, 
            string profileId, 
            decimal profileMargin,
            decimal spread)
        {
            var marginRate = product.GetMarginRate(profileMargin);
            var leverage = new Leverage(marginRate);

            return new TradingInstrument(
                tradingConditionId: profileId,
                instrument: product.ProductId,
                swapLong: TradingInstrumentsConstants.SwapLong,
                swapShort: TradingInstrumentsConstants.SwapShort,
                delta: TradingInstrumentsConstants.Delta,
                dealMinLimit: product.MinOrderSize,
                dealMaxLimit: product.MaxOrderSize,
                positionLimit: product.MaxPositionSize,
                shortPosition: product.ShortPosition,
                liquidationThreshold: TradingInstrumentsConstants.LiquidationThreshold,
                overnightMarginMultiplier: product.OvernightMarginMultiplier,
                commissionRate: TradingInstrumentsConstants.CommissionRate,
                commissionMin: TradingInstrumentsConstants.CommissionMin,
                commissionMax: TradingInstrumentsConstants.CommissionMax,
                commissionCurrency: string.Empty,
                hedgeCost: product.HedgeCost,
                spread: spread,
                initLeverage: leverage,
                maintenanceLeverage: leverage,
                marginRate: marginRate,
                maxPositionNotional: product.MaxPositionNotional
            );
        }
    }
}