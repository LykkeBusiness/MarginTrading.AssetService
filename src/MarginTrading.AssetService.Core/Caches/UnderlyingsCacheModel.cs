using System;

namespace MarginTrading.AssetService.Core.Caches
{
    public class UnderlyingsCacheModel
    {
        public string MdsCode { get; set; }
        public decimal AlmParam { get; set; }
        public string CfiCode { get; set; }
        public bool Eligible871M { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal HedgeCost { get; set; }
        public string Isin { get; set; }
        public DateTime? LastTradingDate { get; set; }
        public string MarketArea { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string Name { get; set; }
        public decimal RepoSurchargePercent { get; set; }
        public decimal Spread { get; set; }
        public string TradingCurrency { get; set; }
        public string CommodityBase { get; set; }
        public string CommodityDetails { get; set; }
        public string BaseCurrency { get; set; }
        public string IndexName { get; set; }
        public string EmirType { get; set; }
        
        public DateTime StartDate { get; set; }
    }
}