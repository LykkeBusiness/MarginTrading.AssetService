using System;
using System.Text;

using Autofac;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Correlation.RabbitMq;
using Lykke.Snow.Mdm.Contracts.Models.Events;

using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Contracts.Messages;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;
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
            builder.AddRabbitMqConnectionProvider();
            
            AddRabbitPublisher<AssetUpsertedEvent>(builder, _settings.LegacyAssetUpdatedRabbitPublisherSettings);
            AddRabbitPublisher<SettingsChangedEvent>(builder, _settings.SettingsChangedRabbitMqSettings);

            var underlyingChangedSubScr = _settings.UnderlyingChangedRabbitSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);

            builder.AddRabbitMqListener<UnderlyingChangedEvent, UnderlyingChangedHandler>(
                    underlyingChangedSubScr,
                    (s, p) =>
                    {
                        var loggerFactory = p.Resolve<ILoggerFactory>();
                        var correlationManager = p.Resolve<RabbitMqCorrelationManager>();

                        s.UseMiddleware(
                                new DeadQueueMiddleware<UnderlyingChangedEvent>(
                                    loggerFactory.CreateLogger<DeadQueueMiddleware<UnderlyingChangedEvent>>()))
                            .UseMiddleware(
                                new ResilientErrorHandlingMiddleware<UnderlyingChangedEvent>(
                                    loggerFactory
                                        .CreateLogger<ResilientErrorHandlingMiddleware<UnderlyingChangedEvent>>(),
                                    TimeSpan.FromSeconds(10),
                                    10))
                            .SetReadHeadersAction(correlationManager.FetchCorrelationIfExists);
                    })
                .AddOptions(RabbitMqListenerOptions<UnderlyingChangedEvent>.MessagePack.NoLoss)
                .AutoStart();
                
            
            var brokerSettingsSubsc = _settings.BrokerSettingsChangedSubscriptionSettings
                .AppendToQueueName($"{_settings.BrokerId}:{_settings.InstanceId}")
                .AppendToDeadLetterExchangeName(_settings.BrokerId);

            builder.AddRabbitMqListener<BrokerSettingsChangedEvent, BrokerSettingsChangedHandler>(
                    brokerSettingsSubsc,
                    (s, p) =>
                    {
                        var loggerFactory = p.Resolve<ILoggerFactory>();
                        var correlationManager = p.Resolve<RabbitMqCorrelationManager>();

                        s.UseMiddleware(
                                new DeadQueueMiddleware<BrokerSettingsChangedEvent>(
                                    loggerFactory.CreateLogger<DeadQueueMiddleware<BrokerSettingsChangedEvent>>()))
                            .UseMiddleware(
                                new ResilientErrorHandlingMiddleware<BrokerSettingsChangedEvent>(
                                    loggerFactory
                                        .CreateLogger<ResilientErrorHandlingMiddleware<BrokerSettingsChangedEvent>>(),
                                    TimeSpan.FromSeconds(10),
                                    10))
                            .SetReadHeadersAction(correlationManager.FetchCorrelationIfExists);
                    })
                .AddOptions(RabbitMqListenerOptions<BrokerSettingsChangedEvent>.MessagePack.NoLoss)
                .AutoStart();
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