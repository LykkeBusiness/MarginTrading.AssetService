using System;
using MarginTrading.SettingsService.Contracts.Enums;

namespace MarginTrading.SettingsService.Contracts.Messages
{
    public class SettingsChangedEvent
    {
        public DateTime Timestamp { get; set; }
        public SettingsTypeContract SettingsType { get; set; }
        public string Route { get; set; }
    }
}