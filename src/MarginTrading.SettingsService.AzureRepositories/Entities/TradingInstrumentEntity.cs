﻿using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class TradingInstrumentEntity : SimpleAzureEntity, ITradingInstrument
    {   
        public override string Id => GetId(TradingConditionId, Instrument);
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
        
        public decimal CommissionRate { get; set; }
        public decimal CommissionMin { get; set; }
        public decimal CommissionMax { get; set; }
        public string CommissionCurrency { get; set; }

        public override void SetKeys()
        {
            this.RowKey = Instrument;
            this.PartitionKey = TradingConditionId;
        }

        public static string GetId(string tradingConditionId, string instrument)
        {
            return $"{tradingConditionId}_{instrument}";
        }
    }
}