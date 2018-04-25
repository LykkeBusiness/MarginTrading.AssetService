using Lykke.SlackNotifications;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;

namespace MarginTrading.SettingsService.Services
{
    public class MaintenanceModeService : IMaintenanceModeService
    {
        private readonly ISlackNotificationsSender _slackNotificationsSender;

        public MaintenanceModeService(ISlackNotificationsSender slackNotificationsSender)
        {
            _slackNotificationsSender = slackNotificationsSender;
        }
        
        private static bool IsEnabled { get; set; }

        public bool CheckIsEnabled()
        {
            return IsEnabled;
        }

        public void SetMode(bool isEnabled)
        {
            IsEnabled = isEnabled;

            _slackNotificationsSender.SendAsync(ChannelTypes.Monitor, $"Backend",
                $"Maintenance mode is {(isEnabled ? "ENABLED" : "DISABLED")}");
        }
    }
}