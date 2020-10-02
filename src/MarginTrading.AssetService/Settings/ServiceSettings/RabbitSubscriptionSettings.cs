using Lykke.RabbitMqBroker.Subscriber;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    public class RabbitSubscriptionSettings
    {
        public string RoutingKey { get; set; }
        public bool IsDurable { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string ConnectionString { get; set; }

        public static implicit operator RabbitMqSubscriptionSettings(RabbitSubscriptionSettings subscriptionSettings)
        {
            return new RabbitMqSubscriptionSettings
            {
                RoutingKey = subscriptionSettings.RoutingKey,
                IsDurable = subscriptionSettings.IsDurable,
                ExchangeName = subscriptionSettings.ExchangeName,
                QueueName = subscriptionSettings.QueueName,
                ConnectionString = subscriptionSettings.ConnectionString
            };
        }
    }
}