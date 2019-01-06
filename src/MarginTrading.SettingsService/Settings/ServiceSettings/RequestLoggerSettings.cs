using JetBrains.Annotations;

namespace MarginTrading.SettingsService.Settings.ServiceSettings
{
    [UsedImplicitly]
    public class RequestLoggerSettings
    {
        public bool Enabled { get; set; }
        public int MaxPartSize { get; set; }
    }
}
