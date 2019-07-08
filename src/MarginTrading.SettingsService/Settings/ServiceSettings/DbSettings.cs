// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

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
