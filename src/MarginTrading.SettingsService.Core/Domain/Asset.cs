using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class Asset : IAsset
    {
        public Asset(string id, string name, int accuracy)
        {
            Id = id;
            Name = name;
            Accuracy = accuracy;
        }

        public string Id { get; }
        public string Name { get; }
        public int Accuracy { get; }
    }
}