// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

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