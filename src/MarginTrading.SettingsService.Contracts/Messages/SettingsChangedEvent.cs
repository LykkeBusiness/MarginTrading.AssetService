using System;
using MarginTrading.SettingsService.Client.Enums;

namespace MarginTrading.SettingsService.Client.Messages
{
    public class SettingsChangedEvent
    {
        public DateTime Timestamp { get; set; }
        public SettingsTypeContract SettingsType { get; set; }
        public string Route { get; set; }
    }
}