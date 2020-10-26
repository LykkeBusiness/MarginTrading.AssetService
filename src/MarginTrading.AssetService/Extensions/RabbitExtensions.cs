using MarginTrading.AssetService.Settings.ServiceSettings;

namespace MarginTrading.AssetService.Extensions
{
    public static class RabbitExtensions
    {
        public static RabbitSubscriptionSettings AppendToQueueName(this RabbitSubscriptionSettings settings, string value)
        {
            settings.QueueName = $"{settings.QueueName}-{value}";

            return settings;
        }
        
        public static RabbitSubscriptionSettings AppendToDeadLetterExchangeName(this RabbitSubscriptionSettings settings, string value)
        {
            settings.DeadLetterExchangeName = $"{settings.DeadLetterExchangeName}-{value}";

            return settings;
        }
    }
}