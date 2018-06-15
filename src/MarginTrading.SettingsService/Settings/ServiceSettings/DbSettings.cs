using Lykke.SettingsReader.Attributes;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        public StorageMode StorageMode { get; set; }
        
        [AzureTableCheck]
        public string LogsAzureConnString { get; set; }
        
        [AzureTableCheck]
        public string AzureConnectionString { get; set; }
        
        [SqlCheck]
        public string SqlConnectionString { get; set; }
    }
}
