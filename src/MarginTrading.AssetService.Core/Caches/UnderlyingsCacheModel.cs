using System;

namespace MarginTrading.AssetService.Core.Caches
{
    public class UnderlyingsCacheModel
    {
        public string MdsCode { get; set; }
        public decimal AlmParam { get; set; }
        public string CfiCode { get; set; }
        public bool Eligible871M { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string Isin { get; set; }
        public DateOnly? LastTradingDate { get; set; }
        public string MarketArea { get; set; }
        public DateOnly? MaturityDate { get; set; }
        public string Name { get; set; }
        public decimal RepoSurchargePercent { get; set; }
        public decimal Spread { get; set; }
        public string TradingCurrency { get; set; }
        public string CommodityBase { get; set; }
        public string CommodityDetails { get; set; }
        public string BaseCurrency { get; set; }
        public string IndexName { get; set; }
        public string EmirType { get; set; }
        
        public DateOnly? StartDate { get; set; }
    }
}