using Lykke.SettingsReader.Attributes;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        public StorageMode StorageMode { get; set; }
        
        [Optional]
        public string LogsAzureConnString { get; set; }
        
        [Optional]
        public string AzureConnectionString { get; set; }
        
        [Optional]
        public string SqlConnectionString { get; set; }
    }
}
