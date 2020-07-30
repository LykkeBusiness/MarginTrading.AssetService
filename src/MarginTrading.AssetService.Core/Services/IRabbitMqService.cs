// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Common;
using Lykke.RabbitMqBroker.Publisher;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IRabbitMqService
    {
        IMessageProducer<TMessage> GetProducer<TMessage>(string connectionString, string exchangeName,
            bool isDurable, IRabbitMqSerializer<TMessage> serializer);

        void Subscribe<TMessage>(string connectionString, string exchangeName, bool isDurable,
            Func<TMessage, Task> handler);

        IRabbitMqSerializer<TMessage> GetJsonSerializer<TMessage>();
        IRabbitMqSerializer<TMessage> GetMsgPackSerializer<TMessage>();
    }
}