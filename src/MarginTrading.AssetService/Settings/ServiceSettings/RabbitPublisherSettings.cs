// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.SettingsReader.Attributes;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    public class RabbitPublisherSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        [Optional] public bool IsDurable { get; set; } = true;

        public static implicit operator RabbitMqSubscriptionSettings(RabbitPublisherSettings publisherSettings)
        {
            return new RabbitMqSubscriptionSettings
            {
                IsDurable = publisherSettings.IsDurable,
                ExchangeName = publisherSettings.ExchangeName,
                ConnectionString = publisherSettings.ConnectionString
            };
        }
    }
}