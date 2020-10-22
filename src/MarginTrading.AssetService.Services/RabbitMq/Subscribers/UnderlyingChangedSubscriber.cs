using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Services.RabbitMq.Handlers;

namespace MarginTrading.AssetService.Services.RabbitMq.Subscribers
{
    public class UnderlyingChangedSubscriber : IStartStop
    {
        private readonly UnderlyingChangedHandler _handler;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<UnderlyingChangedEvent> _subscriber;

        public UnderlyingChangedSubscriber(
            UnderlyingChangedHandler handler,
            RabbitMqSubscriptionSettings settings,
            ILog log)
        {
            _handler = handler;
            _settings = settings;
            _log = log;
        }

        public void Start()
        {
            _subscriber = new RabbitMqSubscriber<UnderlyingChangedEvent>(
                    _settings,
                    new ResilientErrorHandlingStrategy(_log, _settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        retryNum: 10,
                        next: new DeadQueueErrorHandlingStrategy(_log, _settings)))
                .SetMessageDeserializer(new MessagePackMessageDeserializer<UnderlyingChangedEvent>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .SetLogger(_log)
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