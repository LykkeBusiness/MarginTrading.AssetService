using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.SqlRepositories.Entities
{
    public class AssetEntity : IAsset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}