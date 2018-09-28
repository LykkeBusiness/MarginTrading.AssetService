using Lykke.SettingsReader.Attributes;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class DbSettings
    {
        public StorageMode StorageMode { get; set; }
        
        [Optional]
        public string LogsConnString { get; set; }
        
        [Optional]
        public string DataConnString { get; set; }
    }
}
