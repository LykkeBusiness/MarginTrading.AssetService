// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Common;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.AzureRepositories.Entities
{
    public class AssetPairEntity : SimpleAzureEntity, IAssetPair
    {
        public static readonly string Pk = "AssetPairs"; 
        internal override string SimplePartitionKey => Pk;
        
        // Id comes from parent type
        public string Name { get; set; }
        public string BaseAssetId { get; set; }
        public string QuoteAssetId { get; set; }
        public int Accuracy { get; set; }
        public string MarketId { get; set; }
        public string LegalEntity { get; set; }
        public string BasePairId { get; set; }
        MatchingEngineMode IAssetPair.MatchingEngineMode => Enum.Parse<MatchingEngineMode>(MatchingEngineMode);
        public string MatchingEngineMode { get; set; }
        public decimal StpMultiplierMarkupBid { get; set; }
        public decimal StpMultiplierMarkupAsk { get; set; }
        
        public bool IsSuspended { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsDiscontinued { get; set; }
        
        FreezeInfo IAssetPair.FreezeInfo => !string.IsNullOrEmpty(FreezeInfo)
            ? FreezeInfo.DeserializeJson<FreezeInfo>()
            : new FreezeInfo();
        public string FreezeInfo { get; set; }
    }
}