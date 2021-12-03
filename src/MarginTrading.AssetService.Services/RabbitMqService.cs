// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Lykke.Snow.Common.Correlation.RabbitMq;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MarginTrading.AssetService.Services
{
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = {new StringEnumConverter()}
        };

        private readonly ILog _logger;
        private RabbitMqCorrelationManager _correlationManager;
        private ILoggerFactory _loggerFactory;

        private readonly ConcurrentDictionary<string, IStartStop> _subscribers =
            new ConcurrentDictionary<string, IStartStop>();

        private readonly ConcurrentDictionary<RabbitMqSubscriptionSettings, Lazy<IStartStop>> _producers =
            new ConcurrentDictionary<RabbitMqSubscriptionSettings, Lazy<IStartStop>>(
                new SubscriptionSettingsEqualityComparer());

        public RabbitMqService(ILog logger,
            RabbitMqCorrelationManager correlationManager,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _correlationManager = correlationManager;
            _loggerFactory = loggerFactory;
        }

        public void Dispose()
        {
            foreach (var stoppable in _subscribers.Values)
                stoppable.Stop();
            foreach (var stoppable in _producers.Values)
                stoppable.Value.Stop();
        }

        public IRabbitMqSerializer<TMessage> GetJsonSerializer<TMessage>()
        {
            return new JsonMessageSerializer<TMessage>(Encoding.UTF8, JsonSerializerSettings);
        }

        public IRabbitMqSerializer<TMessage> GetMsgPackSerializer<TMessage>()
        {
            return new MessagePackMessageSerializer<TMessage>();
        }

        public IMessageProducer<TMessage> GetProducer<TMessage>(string connectionString, string exchangeName,
            bool isDurable, IRabbitMqSerializer<TMessage> serializer)
        {
            var subscriptionSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = connectionString,
                ExchangeName = exchangeName,
                IsDurable = isDurable,
            };

            return (IMessageProducer<TMessage>) _producers.GetOrAdd(subscriptionSettings, CreateProducer).Value;

            Lazy<IStartStop> CreateProducer(RabbitMqSubscriptionSettings s)
            {
                // Lazy ensures RabbitMqPublisher will be created and started only once
                // https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/
                return new Lazy<IStartStop>(() =>
                {
                    var result = new RabbitMqPublisher<TMessage>(_loggerFactory, s).DisableInMemoryQueuePersistence()
                        .SetSerializer(serializer)
                        .SetWriteHeadersFunc(_correlationManager.BuildCorrelationHeadersIfExists);
                    result.Start();
                    return result;
                });
            }
        }

        public void Subscribe<TMessage>(string connectionString, string exchangeName, bool isDurable,
            Func<TMessage, Task> handler)
        {
            // on-the fly connection strings switch is not supported currently for rabbitMq
            var subscriptionSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = connectionString,
                QueueName =
                    $"{exchangeName}.{PlatformServices.Default.Application.ApplicationName}",
                ExchangeName = exchangeName,
                IsDurable = isDurable,
            };

            var rabbitMqSubscriber = new RabbitMqSubscriber<TMessage>(
                    _loggerFactory.CreateLogger<RabbitMqSubscriber<TMessage>>(),
                    subscriptionSettings)
                .SetMessageDeserializer(new JsonMessageDeserializer<TMessage>(JsonSerializerSettings))
                .Subscribe(handler)
                .UseMiddleware(new ExceptionSwallowMiddleware<TMessage>(
                    _loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TMessage>>()))
                .SetReadHeadersAction(_correlationManager.FetchCorrelationIfExists);

            if (!_subscribers.TryAdd(subscriptionSettings.QueueName, rabbitMqSubscriber))
            {
                throw new InvalidOperationException(
                    $"A subscriber for queue {subscriptionSettings.QueueName} was already initialized");
            }

            rabbitMqSubscriber.Start();
        }

        /// <remarks>
        ///     ReSharper auto-generated
        /// </remarks>
        private sealed class SubscriptionSettingsEqualityComparer : IEqualityComparer<RabbitMqSubscriptionSettings>
        {
            public bool Equals(RabbitMqSubscriptionSettings x, RabbitMqSubscriptionSettings y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.ConnectionString, y.ConnectionString) &&
                       string.Equals(x.ExchangeName, y.ExchangeName);
            }

            public int GetHashCode(RabbitMqSubscriptionSettings obj)
            {
                unchecked
                {
                    return ((obj.ConnectionString != null ? obj.ConnectionString.GetHashCode() : 0) * 397) ^
                           (obj.ExchangeName != null ? obj.ExchangeName.GetHashCode() : 0);
                }
            }
        }
    }
}