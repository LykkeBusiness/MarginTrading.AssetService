using MarginTrading.SettingsService.Contracts.Enums;

namespace MarginTrading.SettingsService.Contracts.AssetPair
{
    /// <summary>
    /// AssetPair update request. IsFrozen and IsDiscontinued will be updated only if != null
    /// </summary>
    public class AssetPairUpdateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BaseAssetId { get; set; }
        public string QuoteAssetId { get; set; }
        public int Accuracy { get; set; }
        public string MarketId { get; set; }
        public string LegalEntity { get; set; }
        public string BasePairId { get; set; }
        public MatchingEngineModeContract MatchingEngineMode { get; set; }
        public decimal StpMultiplierMarkupBid { get; set; }
        public decimal StpMultiplierMarkupAsk { get; set; }
        
        public bool? IsFrozen { get; set; }
        public bool? IsDiscontinued { get; set; }
    }
}