// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class Underlying
    {
        private string _marketId;
        
        [JsonProperty("almParam")]
        public decimal AlmParam { get; set; }

        [JsonProperty("assetType")]
        public string AssetType { get; set; }

        [JsonProperty("cfiCode")]
        public string CfiCode { get; set; }

        [JsonProperty("eligible871m")]
        public bool Eligible871m { get; set; }

        [JsonProperty("hedgeCost")]
        public decimal HedgeCost { get; set; }

        [JsonProperty("isin")]
        public string Isin { get; set; }

        [JsonProperty("market")]
        public string MarketId
        {
            get => MarketDetails?.MarketId ?? _marketId;
            set => _marketId = value;
        }

        [JsonProperty("maturityDate")]
        public string MaturityDate { get; set; }

        [JsonProperty("mdsCode")]
        public string MdsCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("repoSurchargePercent")]
        public decimal RepoSurchargePercent { get; set; }

        [JsonProperty("ricCode")]
        public string RicCode { get; set; }

        [JsonProperty("spread")]
        public decimal Spread { get; set; }

        [JsonProperty("tradingCurrency")]
        public string TradingCurrency { get; set; }

        [JsonProperty("variableInterestRate")]
        public string VariableInterestRate { get; set; }

        [JsonProperty("variableInterestRate1")]
        public string VariableInterestRate1 { get; set; }

        [JsonProperty("variableInterestRate2")]
        public string VariableInterestRate2 { get; set; }

        [JsonProperty("CommodityBase")]
        public string CommodityBase { get; set; }

        [JsonProperty("CommodityDetails")]
        public string CommodityDetails { get; set; }

        [JsonProperty("BaseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("IndexName")]
        public string IndexName { get; set; }

        [JsonProperty("EMIRType")]
        public string EMIRType { get; set; }

        [JsonProperty("UnderlyingCategoryId")]
        public string UnderlyingCategoryId { get; set; }

        /// <summary>
        /// Represents a market object.
        /// Mapped from the normalized data after deserialization.
        /// </summary>
        public Market MarketDetails { get; set; }

        /// <summary>
        /// Represents a collection of interest rate objects.
        /// Mapped from the normalized data after deserialization.
        /// </summary>
        public List<InterestRate> InterestRates { get; set; }

        [JsonProperty("Keywords")]
        public string Keywords { get; set; }

        [JsonIgnore]
        public string[] KeywordsArray =>
            !string.IsNullOrEmpty(Keywords) ? Keywords.Split(',').ToArray() : Array.Empty<string>();

        [JsonProperty("dividendsFactor")]
        public DividendsFactor DividendsFactor { get; set; } = new DividendsFactor();
        
        /// <summary>
        /// List of client profiles with their settings available for this asset type
        /// </summary>
        public List<ClientProfile> AvailableClientProfiles { get; set; }
    }
}