using JetBrains.Annotations;
using MarginTrading.SettingsService.Settings.ServiceSettings;
using MarginTrading.SettingsService.Settings.SlackNotifications;

namespace MarginTrading.SettingsService.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public LykkeServiceSettings LykkeServiceService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
