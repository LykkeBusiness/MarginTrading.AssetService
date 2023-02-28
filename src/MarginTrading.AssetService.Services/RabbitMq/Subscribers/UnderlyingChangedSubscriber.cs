using System;
using System.Threading.Tasks;
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
    public sealed class UnderlyingChangedSubscriber : IStartStop
    {
        private readonly UnderlyingChangedHandler _handler;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<UnderlyingChangedSubscriber> _logger;
        private RabbitMqSubscriber<UnderlyingChangedEvent> _subscriber;
        private readonly RabbitMqCorrelationManager _correlationManager;
        private readonly ILoggerFactory _loggerFactory;

        public UnderlyingChangedSubscriber(
            UnderlyingChangedHandler handler,
            RabbitMqSubscriptionSettings settings,
            RabbitMqCorrelationManager correlationManager,
            ILoggerFactory loggerFactory,
            ILogger<UnderlyingChangedSubscriber> logger)
        {
            _handler = handler;
            _settings = settings;
            _correlationManager = correlationManager;
            _loggerFactory = loggerFactory;
            _logger = logger;
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

            _logger.LogInformation("Handled event {EventName}. Event created at: {Timestamp}",
                nameof(UnderlyingChangedEvent), message.Timestamp.ToShortTimeString());
        }
    }
}