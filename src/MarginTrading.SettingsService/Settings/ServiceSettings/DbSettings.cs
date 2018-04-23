using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
