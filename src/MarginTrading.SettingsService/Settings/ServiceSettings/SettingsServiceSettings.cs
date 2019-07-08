// Copyright (c) 2019 Lykke Corp.

using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;
using MarginTrading.SettingsService.Core.Settings;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SettingsServiceSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitMqSettings SettingsChangedRabbitMqSettings { get; set; }
        
        public DefaultTradingInstrumentSettings TradingInstrumentDefaults { get; set; }
        
        public DefaultLegalEntitySettings LegalEntityDefaults { get; set; }
        
        public CqrsSettings Cqrs { get; set; }
        
        [Optional, CanBeNull]
        public ChaosSettings ChaosKitty { get; set; }
        
        public RequestLoggerSettings RequestLoggerSettings { get; set; }
        
        [Optional]
        public bool UseSerilog { get; set; }

        [Optional]
        public PlatformSettings Platform { get; set; } = new PlatformSettings();
    }
}
