using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class AssetPair : IAssetPair
    {
        public AssetPair(string id, string name, string baseAssetId, string quoteAssetId, int accuracy, string marketId, 
            string legalEntity, string basePairId, MatchingEngineMode matchingEngineMode, 
            decimal stpMultiplierMarkupBid, decimal stpMultiplierMarkupAsk)
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
    }
}