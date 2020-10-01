using Autofac;
using Common;
using Common.Log;
using Cronut.Dto.MessageBus;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.Common.Startup;
using MarginTrading.AssetService.Settings.ServiceSettings;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Modules
{
    public class RabbitMqModule : Module
    {
        private readonly AssetServiceSettings _settings;
        private readonly ILog _log;

        public RabbitMqModule(AssetServiceSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            AddRabbitPublisher<AssetUpsertedEvent>(builder, _settings.LegacyAssetUpdatedRabbitMqSettings);
        }

        private void AddRabbitPublisher<T>(ContainerBuilder builder,
            RabbitMqSettings settings,
            IRabbitMqPublishStrategy rabbitMqPublishStrategy = null,
            IRabbitMqSerializer<T> serializer = null)
        {
            var mappedSettings = MapSettings(settings);
            rabbitMqPublishStrategy = rabbitMqPublishStrategy ?? new DefaultFanoutPublishStrategy(mappedSettings);
            serializer = serializer ?? new JsonMessageSerializer<T>();

            builder.RegisterInstance(
                new RabbitMqPublisher<T>(mappedSettings)
                    .SetSerializer(serializer)
                    .SetLogger(_log)
                    .SetPublishStrategy(rabbitMqPublishStrategy)
                    .DisableInMemoryQueuePersistence())
                .As<IMessageProducer<T>>()
                .As<IStartable>()
                .As<IStopable>();
        }

        private RabbitMqSubscriptionSettings MapSettings(RabbitMqSettings rabbitMqSettings)
        {
            return new RabbitMqSubscriptionSettings
            {
                ConnectionString = rabbitMqSettings.ConnectionString,
                ExchangeName = rabbitMqSettings.ExchangeName,
                IsDurable = rabbitMqSettings.IsDurable,
            };
        }
    }
}