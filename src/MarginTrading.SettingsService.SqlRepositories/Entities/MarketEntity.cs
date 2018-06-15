using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.SqlRepositories.Entities
{
    public class MarketEntity : IMarket
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}