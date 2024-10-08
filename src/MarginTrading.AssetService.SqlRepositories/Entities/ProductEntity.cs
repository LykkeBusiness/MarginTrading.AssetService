using System;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ProductEntity
    {
        // primary id
        public string ProductId { get; set; }
        
        public string AssetTypeId { get; set; }
        public AssetTypeEntity AssetType { get; set; }
        
        public string CategoryId { get; set; }
        
        public ProductCategoryEntity Category { get; set; }
        
        public string Comments { get; set; }
        
        public int ContractSize { get; set; }

        public string IsinLong { get; set; }
        
        public string IsinShort { get; set; }
        
        public string Issuer { get; set; }
        
        public string MarketId { get; set; }

        public MarketSettingsEntity Market { get; set; }

        public string MarketMakerAssetAccountId { get; set; }
        
        public int MaxOrderSize { get; set; }
        
        public int MinOrderSize { get; set; }
        
        public int MaxPositionSize { get; set; }

        public decimal MinOrderDistancePercent { get; set; }

        public string Name { get; set; }
        
        public string NewsId { get; set; }
        
        public string Keywords { get; set; }

        public string PublicationRic { get; set; }

        public string SettlementCurrency { get; set; }

        public bool ShortPosition { get; set; }

        public string TickFormulaId { get; set; }
        
        public TickFormulaEntity TickFormula { get; set; }

        // underlying primary id
        public string UnderlyingMdsCode { get; set; }

        public string ForceId { get; set; }

        public int Parity { get; set; }

        public decimal OvernightMarginMultiplier { get; set; }
        
        public string TradingCurrencyId { get; set;}
        
        public CurrencyEntity TradingCurrency { get; set; }
        
        public DateOnly? StartDate { get; set; }
        public DateOnly? ActualDiscontinuedDate { get; set; }
        
        public bool IsSuspended { get; set; }
        
        public bool IsFrozen { get; set; }
        
        public string FreezeInfo { get; set; }
        
        public bool IsDiscontinued { get; set; }
        
        public bool IsStarted { get; set; }
        
        public byte[] Timestamp { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public decimal HedgeCost { get; set; }

        public bool EnforceMargin { get; set; }

        public decimal? Margin { get; set; }
        
        public decimal? MaxPositionNotional { get; set; }

        public bool IsTradingDisabled { get; set; }
    }
}