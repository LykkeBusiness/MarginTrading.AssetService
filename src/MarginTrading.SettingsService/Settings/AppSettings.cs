using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;
using MarginTrading.SettingsService.Settings.ServiceSettings;
using MarginTrading.SettingsService.Settings.SlackNotifications;

namespace MarginTrading.SettingsService.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public MarginTradingSettingsServiceSettings MarginTradingSettingsService { get; set; }
        [Optional, CanBeNull] public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}