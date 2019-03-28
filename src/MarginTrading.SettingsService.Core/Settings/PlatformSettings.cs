using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Core.Settings
{
    public class PlatformSettings
    {
        [Optional]
        public string PlatformMarketId { get; set; } = "PlatformScheduleMarketId";
    }
}