using System.Collections.Generic;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common.Converters;
using Lykke.Snow.Common.Types;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Core.Domain
{
    public class Product : IAuditableObject<AuditDataType>
    {
        // primary id
        public string ProductId { get; set; }

        public string AssetType { get; set; }

        public string Category { get; set; }

        public string Comments { get; set; }

        public ContractSize ContractSize { get; set; }

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
        
        public DateOnly? StartDate { get; set; }

        public bool IsSuspended { get; set; }

        public bool IsFrozen { get; set; }

        public ProductFreezeInfo FreezeInfo { get; set; }

        public bool IsDiscontinued { get; set; }

        public bool IsStarted { get; set; }

        public byte[] Timestamp { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public decimal HedgeCost { get; set; }

        public bool EnforceMargin { get; set; }

        public decimal? Margin { get; set; }

        public bool IsTradingDisabled { get; set; }

        public Product ShallowCopy()
        {
            return (Product) this.MemberwiseClone();
        }

        public AuditDataType GetAuditDataType() => AuditDataType.Product;

        public string GetAuditReference() => ProductId;

        public string ToAuditJson() =>
            JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Converters = new List<JsonConverter> { new DateOnlyAuditConverter() }
                });
    }
}