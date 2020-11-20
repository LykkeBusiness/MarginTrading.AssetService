using System;
using MarginTrading.AssetService.Core.Extensions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class Product
    {
        // primary id
        public string ProductId { get; set; }

        public string AssetType { get; set; }

        public string Category { get; set; }

        public string Comments { get; set; }

        public int ContractSize { get; set; }

        public string IsinLong { get; set; }

        public string IsinShort { get; set; }

        public string Issuer { get; set; }

        public string Market { get; set; }

        public string MarketMakerAssetAccountId { get; set; }

        public int MaxOrderSize { get; set; }

        public int MinOrderSize { get; set; }

        public int MaxPositionSize { get; set; }

        public decimal MinOrderDistancePercent { get; set; }

        public decimal MinOrderEntryInterval { get; set; }

        public string Name { get; set; }

        public string NewsId { get; set; }

        public string Keywords { get; set; }

        public string PublicationRic { get; set; }

        public string SettlementCurrency { get; set; }

        public bool ShortPosition { get; set; }

        public string Tags { get; set; }

        public string TickFormula { get; set; }

        // underlying primary id
        public string UnderlyingMdsCode { get; set; }

        public string ForceId { get; set; }

        public int Parity { get; set; }

        public decimal OvernightMarginMultiplier { get; set; }

        public string TradingCurrency { get; set; }

        private DateTime? _startDate;

        public DateTime? StartDate
        {
            get => _startDate;
            set => _startDate = value?.TrimMilliseconds();
        }

        public bool IsSuspended { get; set; }

        public bool IsFrozen { get; set; }

        public ProductFreezeInfo FreezeInfo { get; set; }

        public bool IsDiscontinued { get; set; }

        public bool IsStarted { get; set; }

        public byte[] Timestamp { get; set; }

        public Product ShallowCopy()
        {
            return (Product) this.MemberwiseClone();
        }
    }
}