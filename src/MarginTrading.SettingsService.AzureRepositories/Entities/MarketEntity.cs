using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class MarketEntity : SimpleAzureEntity, IMarket
    {
        internal override string SimplePartitionKey => "Markets";
        
        // Id comes from parent type
        public string Name { get; set; }
    }
}