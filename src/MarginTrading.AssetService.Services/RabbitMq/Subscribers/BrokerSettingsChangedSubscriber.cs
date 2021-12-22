using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Lykke.Snow.Common.Correlation.RabbitMq;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;
using Microsoft.Extensions.Logging;
using IStartStop = Lykke.RabbitMqBroker.IStartStop;

namespace MarginTrading.AssetService.Services.RabbitMq.Subscribers
{
    public class BrokerSettingsChangedSubscriber : IStartStop
    {
        private readonly BrokerSettingsChangedHandler _handler;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<BrokerSettingsChangedEvent> _subscriber;
        private RabbitMqCorrelationManager _correlationManager;
        private ILoggerFactory _loggerFactory;

        public BrokerSettingsChangedSubscriber(
            BrokerSettingsChangedHandler handler,
            RabbitMqSubscriptionSettings settings,
            ILog log,
            RabbitMqCorrelationManager correlationManager,
            ILoggerFactory loggerFactory)
        {
            _handler = handler;
            _settings = settings;
            _log = log;
            _correlationManager = correlationManager;
            _loggerFactory = loggerFactory;
        }

        public void Start()
        {
            _subscriber = new RabbitMqSubscriber<BrokerSettingsChangedEvent>(
                    _loggerFactory.CreateLogger<RabbitMqSubscriber<BrokerSettingsChangedEvent>>(),
                    _settings)
                .SetMessageDeserializer(new MessagePackMessageDeserializer<BrokerSettingsChangedEvent>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .UseMiddleware(new DeadQueueMiddleware<BrokerSettingsChangedEvent>(
                    _loggerFactory.CreateLogger<DeadQueueMiddleware<BrokerSettingsChangedEvent>>()))
                .UseMiddleware(new ResilientErrorHandlingMiddleware<BrokerSettingsChangedEvent>(
                    _loggerFactory.CreateLogger<ResilientErrorHandlingMiddleware<BrokerSettingsChangedEvent>>(),
                    TimeSpan.FromSeconds(10),
                    10))
                .SetReadHeadersAction(_correlationManager.FetchCorrelationIfExists)
                .CreateDefaultBinding()
                .Subscribe(ProcessMessageAsync)
                .Start();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            if (_subscriber != null)
            {
                _subscriber.Stop();
                _subscriber.Dispose();
                _subscriber = null;
            }
        }

        private async Task ProcessMessageAsync(BrokerSettingsChangedEvent message)
        {
            await _handler.Handle(message);

            _log.Info($"Handled event {nameof(BrokerSettingsChangedEvent)}. Event created at: {message.Timestamp.ToShortTimeString()}");
        }
    }
}