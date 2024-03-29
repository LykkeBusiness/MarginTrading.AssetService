﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Linq;

using Lykke.Snow.Domain.Assets;

using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class Asset
    {
        private decimal _overnightMarginMultiplier;

        [JsonProperty("category")]
        public string CategoryRaw { get; set; }

        [JsonProperty("categoryBottomLevel")]
        public string CategoryBottomLevel
        {
            get => CategoryRaw.Trim('/').Split('/').LastOrDefault();
            set { } // This empty setter is needed for the autogenerated proxy clients to be able to handle the deserialization of responses.
        }

        [JsonProperty("categoryTopLevel")]
        public string CategoryTopLevel
        {
            get => CategoryRaw.Trim('/').Split('/').FirstOrDefault();
            set { } // This empty setter is needed for the autogenerated proxy clients to be able to handle the deserialization of responses.
        }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("contractSize")]
        public int ContractSize { get; set; }

        [JsonProperty("expiryDate")]
        public string ExpiryDate { get; set; }

        [JsonProperty("isinLong")]
        public string IsinLong { get; set; }

        [JsonProperty("isinShort")]
        public string IsinShort { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("lastTradingDate")]
        public string LastTradingDate { get; set; }

        [JsonProperty("marketMakerAssetAccountId")]
        public string MarketMakerAssetAccountId { get; set; }

        [JsonProperty("maxOrderSize")]
        public int MaxOrderSize { get; set; }

        [JsonProperty("maxPositionSize")]
        public int MaxPositionSize { get; set; }

        [JsonProperty("minOrderDistancePercent")]
        public decimal MinOrderDistancePercent { get; set; }

        [JsonProperty("minOrderSize")]
        public int MinOrderSize { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("newsId")]
        public string NewsId { get; set; }

        [JsonProperty("productId")]
        public ProductId AssetId { get; set; }

        [JsonProperty("publicationRic")]
        public string PublicationRic { get; set; }

        [JsonProperty("settlementCurrency")]
        public string SettlementCurrency { get; set; }

        [JsonProperty("shortPosition")]
        public bool ShortPosition { get; set; }

        [JsonProperty("tickFormula")]
        public string TickFormulaName { get; set; }

        [JsonProperty("underlyingMdsCode")]
        public string UnderlyingMdsCode { get; set; }

        [JsonProperty("forceId")]
        public string ForceId { get; set; }

        [DefaultValue(1)]
        [JsonProperty("parity", DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Parity { get; set; }

        [JsonProperty("liquidationThresholdQty")]
        public decimal LiquidationThresholdQuantity { get; set; }

        [JsonProperty("overnightMarginMultiplier")]
        public decimal OvernightMarginMultiplier
        {
            get => _overnightMarginMultiplier > 1 ? _overnightMarginMultiplier : 1;
            set => _overnightMarginMultiplier = value;
        }

        /// <summary>
        /// Represents an underlying object.
        /// Mapped from the normalized data after deserialization.
        /// </summary>
        public Underlying Underlying { get; set; }

        /// <summary>
        /// Represents a tick formula object.
        /// Mapped from the normalized data after deserialization.
        /// </summary>
        public TickFormula TickFormulaDetails { get; set; }

        /// <summary>
        /// Gets or sets the DisplayPrecision (accuracy).
        /// It's currently hardcoded as per the requirements.
        /// </summary>
        public int DisplayPrecision { get; set; } = 5;

        public DividendsFactor DividendsFactor { get; set; } = new DividendsFactor();
        
        [JsonProperty("enforceMargin")]
        public bool EnforceMargin { get; set; }
        
        [JsonProperty("margin")]
        public decimal? Margin { get; set; }
        
        [JsonProperty("maxPositionNotional")]
        public decimal? MaxPositionNotional { get; set; }
    }
}