
using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        [Optional] public bool IsDurable { get; set; } = true;
    }
}