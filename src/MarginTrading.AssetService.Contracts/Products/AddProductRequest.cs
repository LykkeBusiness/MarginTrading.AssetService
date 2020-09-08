using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Core;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class AddProductRequest : UserRequest
    {
        // primary id
        [MaxLength(400)]
        public string ProductId { get; set; }
        
        [MaxLength(400)]
        public string AssetType { get; set; }
        
        [MaxLength(400)]
        public string Category { get; set; }
        
        [MaxLength(400)]
        public string Comments { get; set; }
        
        public int ContractSize { get; set; }

        [MaxLength(400)]
        public string IsinLong { get; set; }
        
        [MaxLength(400)]
        public string IsinShort { get; set; }
        
        [MaxLength(400)]
        public string Issuer { get; set; }
        
        [MaxLength(400)]
        public string Market { get; set; }
        
        [MaxLength(400)]
        public string MarketMakerAssetAccountId { get; set; }
        
        public int MaxOrderSize { get; set; }
        
        public int MinOrderSize { get; set; }
        
        public int MaxPositionSize { get; set; }

        public decimal MinOrderDistancePercent { get; set; }
        
        public decimal MinOrderEntryInterval { get; set; }

        [MaxLength(400)]
        public string Name { get; set; }
        
        [MaxLength(400)]
        public string NewsId { get; set; }
        
        [MaxLength(400)]
        public string Keywords { get; set; }

        [MaxLength(400)]
        public string PublicationRic { get; set; }

        [MaxLength(400)]
        public string SettlementCurrency { get; set; }

        public bool ShortPosition { get; set; }

        [MaxLength(400)]
        public string Tags { get; set; }

        [MaxLength(400)]
        public string TickFormula { get; set; }

        // underlying primary id
        [MaxLength(100)]
        public string UnderlyingMdsCode { get; set; }

        [MaxLength(400)]
        public string ForceId { get; set; }

        public int Parity { get; set; }

        public decimal OvernightMarginMultiplier { get; set; }
    }
}