using Common.Log;

namespace MarginTrading.SettingsService.Services
{
    public static class LogLocator
    {
        public static ILog CommonLog { get; set; }
        
        public static ILog RequestsLog { get; set; }
    }
}