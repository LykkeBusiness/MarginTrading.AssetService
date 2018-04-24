using MarginTrading.SettingsService.StorageInterfaces.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class MarketEntity : SimpleAzureEntity, IMarketEntity
    {
        public MarketEntity()
        {
            PartitionKey = "Markets";
        }
        
        // Id comes from parent type
        public string Name { get; set; }
    }
}