using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        public string StorageMode { get; set; }
        
        [AzureTableCheck]
        public string LogsConnString { get; set; }
        [AzureTableCheck]
        public string MarginTradingConnString { get; set; }
        
        [SqlCheck]
        public string SqlConnectionString { get; set; }
    }
}
