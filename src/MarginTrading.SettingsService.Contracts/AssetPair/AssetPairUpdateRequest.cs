// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Contracts.Enums;

namespace MarginTrading.SettingsService.Contracts.AssetPair
{
    /// <summary> 
    /// AssetPair update request. Properties are updated only if value != null. 
    /// </summary> 
    public class AssetPairUpdateRequest 
    { 
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string BaseAssetId { get; set; } 
        public string QuoteAssetId { get; set; } 
        public int? Accuracy { get; set; } 
        public string MarketId { get; set; } 
        public string LegalEntity { get; set; } 
        public string BasePairId { get; set; } 
        public MatchingEngineModeContract? MatchingEngineMode { get; set; } 
        public decimal? StpMultiplierMarkupBid { get; set; } 
        public decimal? StpMultiplierMarkupAsk { get; set; } 
         
        public bool? IsFrozen { get; set; } 
        public bool? IsDiscontinued { get; set; } 
        public FreezeInfoContract FreezeInfo { get; set; }
    } 
}