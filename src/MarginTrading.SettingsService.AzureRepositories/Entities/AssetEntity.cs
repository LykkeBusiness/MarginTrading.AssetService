using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class AssetEntity : SimpleAzureEntity, IAsset
    {
        internal override string SimplePartitionKey => "Assets";

        // Id comes from parent type
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}