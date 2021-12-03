using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
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
    public class UnderlyingChangedSubscriber : IStartStop
    {
        private readonly UnderlyingChangedHandler _handler;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<UnderlyingChangedEvent> _subscriber;
        private RabbitMqCorrelationManager _correlationManager;
        private ILoggerFactory _loggerFactory;

        public UnderlyingChangedSubscriber(
            UnderlyingChangedHandler handler,
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
            _subscriber = new RabbitMqSubscriber<UnderlyingChangedEvent>(
                    _loggerFactory.CreateLogger<RabbitMqSubscriber<UnderlyingChangedEvent>>(),
                    _settings)
                .SetMessageDeserializer(new MessagePackMessageDeserializer<UnderlyingChangedEvent>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .UseMiddleware(new DeadQueueMiddleware<UnderlyingChangedEvent>(
                    _loggerFactory.CreateLogger<DeadQueueMiddleware<UnderlyingChangedEvent>>()))
                .UseMiddleware(new ResilientErrorHandlingMiddleware<UnderlyingChangedEvent>(
                    _loggerFactory.CreateLogger<ResilientErrorHandlingMiddleware<UnderlyingChangedEvent>>(),
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

        private async Task ProcessMessageAsync(UnderlyingChangedEvent message)
        {
            await _handler.Handle(message);

            _log.WriteInfo(nameof(UnderlyingChangedSubscriber), nameof(ProcessMessageAsync),
                $"Handled event {nameof(UnderlyingChangedEvent)}. Event created at: {message.Timestamp.ToShortTimeString()}");
        }
    }
}