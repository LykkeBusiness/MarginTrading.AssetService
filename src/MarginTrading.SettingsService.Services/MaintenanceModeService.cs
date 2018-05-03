using Lykke.SlackNotifications;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;

namespace MarginTrading.SettingsService.Services
{
    public class MaintenanceModeService : IMaintenanceModeService
    {
        private static bool IsEnabled { get; set; }

        public bool CheckIsEnabled()
        {
            return IsEnabled;
        }

        public void SetMode(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}