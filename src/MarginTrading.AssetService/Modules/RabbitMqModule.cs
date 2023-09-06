using System.Text;

using Autofac;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Correlation.RabbitMq;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Contracts.Messages;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;
using MarginTrading.AssetService.Services.RabbitMq.Subscribers;
using MarginTrading.AssetService.Settings.ServiceSettings;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly AssetServiceSettings _settings;
        
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = {new StringEnumConverter()}
        };

        public RabbitMqModule(IReloadingManager<AssetServiceSettings> settings)
        {
            _settings = settings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            AddRabbitPublisher<AssetUpsertedEvent>(builder, _settings.LegacyAssetUpdatedRabbitPublisherSettings);
            AddRabbitPublisher<SettingsChangedEvent>(builder, _settings.SettingsChangedRabbitMqSettings);

            var underlyingChangedSubScr = _settings.UnderlyingChangedRabbitSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);
            
            builder.Register(x => new UnderlyingChangedSubscriber(x.Resolve<UnderlyingChangedHandler>(),
                    underlyingChangedSubScr,
                    x.Resolve<RabbitMqCorrelationManager>(),
                    x.Resolve<ILoggerFactory>(),
                    x.Resolve<ILogger<UnderlyingChangedSubscriber>>()))
                .As<IStartStop>()
                .SingleInstance();
            
            var brokerSettingsSubsc = _settings.BrokerSettingsChangedSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);

            builder.Register(x => new BrokerSettingsChangedSubscriber(x.Resolve<BrokerSettingsChangedHandler>(),
                    brokerSettingsSubsc,
                    x.Resolve<RabbitMqCorrelationManager>(),
                    x.Resolve<ILoggerFactory>(),
                    x.Resolve<ILogger<BrokerSettingsChangedSubscriber>>()))
                .As<IStartStop>()
                .SingleInstance();
        }

        private void AddRabbitPublisher<T>(ContainerBuilder builder,
            RabbitPublisherSettings settings,
            IRabbitMqPublishStrategy rabbitMqPublishStrategy = null,
            IRabbitMqSerializer<T> serializer = null)
        {
            rabbitMqPublishStrategy ??= new DefaultFanoutPublishStrategy(settings);
            serializer ??= new JsonMessageSerializer<T>(Encoding.UTF8, JsonSerializerSettings);

            builder.Register(x =>
                    new RabbitMqPublisher<T>(x.Resolve<ILoggerFactory>(), settings)
                        .SetSerializer(serializer)
                        .SetWriteHeadersFunc(x.Resolve<RabbitMqCorrelationManager>().BuildCorrelationHeadersIfExists)
                        .SetPublishStrategy(rabbitMqPublishStrategy)
                        .DisableInMemoryQueuePersistence())
                .As<IMessageProducer<T>>()
                .As<IStartStop>()
                .SingleInstance();
        }
    }
}