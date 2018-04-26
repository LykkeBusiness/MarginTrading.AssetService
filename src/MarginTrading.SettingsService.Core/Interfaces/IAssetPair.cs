namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IAssetPair
    {
         string Id { get; }
         string Name { get; }
         string BaseAssetId { get; }
         string QuoteAssetId { get; }
         int Accuracy { get; }
         string MarketId { get; }
         string LegalEntity { get; }
         string BasePairId { get; }
         string MatchingEngineMode { get; }
         decimal StpMultiplierMarkupBid { get; }
         decimal StpMultiplierMarkupAsk { get; }
    }
}
