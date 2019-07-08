// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class AssetPair : IAssetPair
    {
        public AssetPair(string id, string name, string baseAssetId, string quoteAssetId, int accuracy, string marketId, 
            string legalEntity, string basePairId, MatchingEngineMode matchingEngineMode, 
            decimal stpMultiplierMarkupBid, decimal stpMultiplierMarkupAsk, 
            bool isSuspended, bool isFrozen, bool isDiscontinued)
        {
            Id = id;
            Name = name;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            Accuracy = accuracy;
            MarketId = marketId;
            LegalEntity = legalEntity;
            BasePairId = basePairId;
            MatchingEngineMode = matchingEngineMode;
            StpMultiplierMarkupBid = stpMultiplierMarkupBid;
            StpMultiplierMarkupAsk = stpMultiplierMarkupAsk;
            
            IsSuspended = isSuspended;
            IsFrozen = isFrozen;
            IsDiscontinued = isDiscontinued;
        }

        public string Id { get; }
        public string Name { get; }
        public string BaseAssetId { get; }
        public string QuoteAssetId { get; }
        public int Accuracy { get; }
        public string MarketId { get; }
        public string LegalEntity { get; }
        public string BasePairId { get; }
        public MatchingEngineMode MatchingEngineMode { get; }
        public decimal StpMultiplierMarkupBid { get; }
        public decimal StpMultiplierMarkupAsk { get; }
        
        public bool IsSuspended { get; }
        public bool IsFrozen { get; }
        public bool IsDiscontinued { get; }

        public IAssetPair CreateForUpdate(bool isSuspended)
        {
            return new AssetPair(
                id: this.Id,
                name: this.Name,
                baseAssetId: this.BaseAssetId,
                quoteAssetId: this.QuoteAssetId,
                accuracy: this.Accuracy,
                marketId: this.MarketId,
                legalEntity: this.LegalEntity,
                basePairId: this.BasePairId,
                matchingEngineMode: this.MatchingEngineMode,
                stpMultiplierMarkupBid: this.StpMultiplierMarkupBid,
                stpMultiplierMarkupAsk: this.StpMultiplierMarkupAsk,
                
                isSuspended: isSuspended,
                isFrozen: this.IsFrozen,
                isDiscontinued: this.IsDiscontinued
            );
        }
    }
}