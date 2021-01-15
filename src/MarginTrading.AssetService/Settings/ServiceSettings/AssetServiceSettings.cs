// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Core.Settings.Rates;
using MarginTrading.AssetService.Settings.Candles;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AssetServiceSettings
    {
        public DbSettings Db { get; set; }
        
        public RabbitPublisherSettings SettingsChangedRabbitMqSettings { get; set; }
        
        public DefaultTradingInstrumentSettings TradingInstrumentDefaults { get; set; }
        
        public DefaultLegalEntitySettings LegalEntityDefaults { get; set; }
        
        public CqrsSettings Cqrs { get; set; }
        
        [Optional, CanBeNull]
        public ChaosSettings ChaosKitty { get; set; }
        
        public DefaultRateSettings DefaultRateSettings { get; set; }
        
        public RequestLoggerSettings RequestLoggerSettings { get; set; }
        
        [Optional]
        public bool UseSerilog { get; set; }

        [Optional]
        public PlatformSettings Platform { get; set; } = new PlatformSettings();
        
        [Optional]
        public CandlesShardingSettings CandlesSharding { get; set; }

        public ServiceSettings MdmService { get; set; }

        [Optional]
        public DefaultTradingConditionsSettings TradingConditionsDefaults { get; set; } = new DefaultTradingConditionsSettings();

        public string BrokerId { get; set; }
        public string InstanceId { get; set; }
        public RabbitPublisherSettings LegacyAssetUpdatedRabbitPublisherSettings { get; set; }
        public RabbitSubscriptionSettings UnderlyingChangedRabbitSubscriptionSettings { get; set; }
        
        [Optional]
        public List<string> AssetTypesWithZeroInterestRates { get; set; } = new List<string>();
    }
}
