using System;
using Autofac;
using Common;
using Common.Log;
using Cronut.Dto.MessageBus;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Startup;
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

            var settings = _settings.UnderlyingChangedRabbitSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);
            
            builder.Register(x => new UnderlyingChangedSubscriber(x.Resolve<UnderlyingChangedHandler>(),
                    settings, _log))
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

            builder.RegisterInstance(
                    new RabbitMqPublisher<T>(settings)
                        .SetSerializer(serializer)
                        .SetLogger(_log)
                        .SetPublishStrategy(rabbitMqPublishStrategy)
                        .DisableInMemoryQueuePersistence())
                .As<IMessageProducer<T>>()
                .As<IStartStop>();
        }
    }
}