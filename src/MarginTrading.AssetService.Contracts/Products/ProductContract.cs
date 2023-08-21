using System;
using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.Products
{
    [MessagePackObject()]
    public class ProductContract
    {
        // primary id
        [MessagePack.Key(0)]
        public string ProductId { get; set; }
        
        [MessagePack.Key(1)]
        public string AssetType { get; set; }
        
        [Required]
        [MessagePack.Key(2)]
        public string Category { get; set; }
        
        [MessagePack.Key(3)]
        public string Comments { get; set; }
        
        [MessagePack.Key(4)]
        public int ContractSize { get; set; }

        [MessagePack.Key(5)]
        public string IsinLong { get; set; }
        
        [MessagePack.Key(6)]
        public string IsinShort { get; set; }
        
        [MessagePack.Key(7)]
        public string Issuer { get; set; }
        
        [MessagePack.Key(8)]
        public string Market { get; set; }
        
        [MessagePack.Key(9)]
        public string MarketMakerAssetAccountId { get; set; }
        
        [MessagePack.Key(10)]
        public int MaxOrderSize { get; set; }
        
        [MessagePack.Key(11)]
        public int MinOrderSize { get; set; }
        
        [MessagePack.Key(12)]
        public int MaxPositionSize { get; set; }

        [MessagePack.Key(13)]
        public decimal MinOrderDistancePercent { get; set; }

        [MessagePack.Key(15)]
        public string Name { get; set; }
        
        [MessagePack.Key(16)]
        public string NewsId { get; set; }
        
        [MessagePack.Key(17)]
        public string Keywords { get; set; }

        [MessagePack.Key(18)]
        public string PublicationRic { get; set; }

        [MessagePack.Key(19)]
        public string SettlementCurrency { get; set; }

        [MessagePack.Key(20)]
        public bool ShortPosition { get; set; }

        [MessagePack.Key(22)]
        public string TickFormula { get; set; }

        // underlying primary id
        [Required]
        [MessagePack.Key(23)]
        public string UnderlyingMdsCode { get; set; }

        [MessagePack.Key(24)]
        public string ForceId { get; set; }

        [MessagePack.Key(25)]
        public int Parity { get; set; }

        [MessagePack.Key(26)]
        public decimal OvernightMarginMultiplier { get; set; }

        [MessagePack.Key(27)]
        public string TradingCurrency { get; set;}

        [MessagePack.Key(28)]
        public bool IsSuspended { get; set; }
        
        [MessagePack.Key(29)]
        public bool IsFrozen { get; set; }
        
        [MessagePack.Key(30)]
        public ProductFreezeInfoContract FreezeInfo { get; set; }
        
        [MessagePack.Key(31)]
        public bool IsDiscontinued { get; set; }

        [MessagePack.Key(32)]
        public DateTime StartDate { get; set; }

        [MessagePack.Key(33)]
        public bool IsStarted { get; set; }

        [MessagePack.Key(34)]
        public decimal? DividendsLong { get; set; }

        [MessagePack.Key(35)]
        public decimal? DividendsShort { get; set; }

        [MessagePack.Key(36)]
        public decimal? Dividends871M { get; set; }

        [MessagePack.Key(37)]
        public decimal HedgeCost { get; set; }

        [MessagePack.Key(38)]
        public bool EnforceMargin { get; set; }

        [MessagePack.Key(39)]
        public decimal? Margin { get; set; }

        [MessagePack.Key(40)]
        public bool IsTradingDisabled { get; set; }

        [MessagePack.Key(41)]
        public decimal? MaxPositionNotional { get; set; }
    }
}