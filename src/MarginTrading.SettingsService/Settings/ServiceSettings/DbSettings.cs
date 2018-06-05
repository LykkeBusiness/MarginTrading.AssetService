using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string MarginTradingConnString { get; set; }
    }
}
