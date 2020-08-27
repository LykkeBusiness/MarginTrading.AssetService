using Lykke.SettingsReader.Attributes;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    public class ServiceSettings
    {
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }

        [Optional]
        public string ApiKey { get; set; }
    }
}
