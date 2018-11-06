using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class TradingInstrument : ITradingInstrument
    {
        public TradingInstrument(string tradingConditionId, string instrument, int leverageInit, 
            int leverageMaintenance, decimal swapLong, decimal swapShort, decimal delta, decimal dealMinLimit, 
            decimal dealMaxLimit, decimal positionLimit, decimal liquidationThreshold, 
            decimal commissionRate, decimal commissionMin, decimal commissionMax, string commissionCurrency)
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
            LiquidationThreshold = liquidationThreshold;
            
            CommissionRate = commissionRate;
            CommissionMin = commissionMin;
            CommissionMax = commissionMax;
            CommissionCurrency = commissionCurrency;
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
        public decimal LiquidationThreshold { get; }
        
        public decimal CommissionRate { get; }
        public decimal CommissionMin { get; }
        public decimal CommissionMax { get; }
        public string CommissionCurrency { get; }
    }
}