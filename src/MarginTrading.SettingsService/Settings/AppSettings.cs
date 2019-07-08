// Copyright (c) 2019 Lykke Corp.

using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;
using Lykke.Snow.Common.Startup.ApiKey;
using MarginTrading.SettingsService.Settings.ServiceSettings;
using MarginTrading.SettingsService.Settings.SlackNotifications;

namespace MarginTrading.SettingsService.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public SettingsServiceSettings MarginTradingSettingsService { get; set; }
        
        [Optional, CanBeNull]
        public ClientSettings MarginTradingSettingsServiceClient { get; set; } = new ClientSettings();
            
        [Optional, CanBeNull] public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}