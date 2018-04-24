using MarginTrading.SettingsService.StorageInterfaces.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class AssetPairEntity : SimpleAzureEntity, IAssetPairEntity
    {
        public AssetPairEntity()
        {
            this.PartitionKey = "AssetPairs";
        }
        
        // Id comes from parent type
        public string Name { get; set; }
        public string BaseAssetId { get; set; }
        public string QuoteAssetId { get; set; }
        public int Accuracy { get; set; }
        public string MarketId { get; set; }
        public string LegalEntity { get; set; }
        public string BasePairId { get; set; }
        public string MatchingEngineMode { get; set; }
        public decimal StpMultiplierMarkupBid { get; set; }
        public decimal StpMultiplierMarkupAsk { get; set; }
    }
}