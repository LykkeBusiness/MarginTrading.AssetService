// Copyright (c) 2019 Lykke Corp.

using JetBrains.Annotations;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    [UsedImplicitly]
    public class RequestLoggerSettings
    {
        public bool Enabled { get; set; }
        
        public bool EnabledForGet { get; set; }
        
        public int MaxPartSize { get; set; }
    }
}
