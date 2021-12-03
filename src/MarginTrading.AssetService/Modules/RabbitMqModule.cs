using Autofac;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Correlation.RabbitMq;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;
using MarginTrading.AssetService.Services.RabbitMq.Subscribers;
using MarginTrading.AssetService.Settings.ServiceSettings;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly AssetServiceSettings _settings;
        private readonly ILog _log;

        public RabbitMqModule(IReloadingManager<AssetServiceSettings> settings, ILog log)
        {
            _settings = settings.CurrentValue;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            AddRabbitPublisher<AssetUpsertedEvent>(builder, _settings.LegacyAssetUpdatedRabbitPublisherSettings);

            var underlyingChangedSubScr = _settings.UnderlyingChangedRabbitSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);
            
            builder.Register(x => new UnderlyingChangedSubscriber(x.Resolve<UnderlyingChangedHandler>(),
                    underlyingChangedSubScr, _log, x.Resolve<RabbitMqCorrelationManager>(), x.Resolve<ILoggerFactory>()))
                .As<IStartStop>()
                .SingleInstance();
            
            var brokerSettingsSubsc = _settings.BrokerSettingsChangedSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);

            builder.Register(x => new BrokerSettingsChangedSubscriber(x.Resolve<BrokerSettingsChangedHandler>(),
                    brokerSettingsSubsc, _log, x.Resolve<RabbitMqCorrelationManager>(), x.Resolve<ILoggerFactory>()))
                .As<IStartStop>()
                .SingleInstance();
        }

        private void AddRabbitPublisher<T>(ContainerBuilder builder,
            RabbitPublisherSettings settings,
            IRabbitMqPublishStrategy rabbitMqPublishStrategy = null,
            IRabbitMqSerializer<T> serializer = null)
        {
            rabbitMqPublishStrategy = rabbitMqPublishStrategy ?? new DefaultFanoutPublishStrategy(settings);
            serializer = serializer ?? new JsonMessageSerializer<T>();

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