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
    }
}
