using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Constants
{
    public static class AssetPairConstants
    {
        public const int Accuracy = 5;
        public const string BasePairId = null;
        public const MatchingEngineMode MatchingEngineMode = (MatchingEngineMode)2;
        public const decimal StpMultiplierMarkupBid = 1;
        public const decimal StpMultiplierMarkupAsk = 1;
        public const string BaseCurrencyId = "EUR";
        public const string FxMarketId = "FxMarket";
    }
}