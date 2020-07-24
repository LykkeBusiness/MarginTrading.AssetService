// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using JetBrains.Annotations;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    [UsedImplicitly]
    public class RequestLoggerSettings
    {
        public bool Enabled { get; set; }
        
        public bool EnabledForGet { get; set; }
        
        public int MaxPartSize { get; set; }
    }
}
