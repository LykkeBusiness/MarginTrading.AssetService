using MarginTrading.SettingsService.StorageInterfaces.Entities;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class AssetEntity : SimpleAzureEntity, IAssetEntity
    {
        public AssetEntity()
        {
            PartitionKey = "Assets";
        }
        
        // Id comes from parent type
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}