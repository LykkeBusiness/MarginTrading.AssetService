// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Enums;

namespace MarginTrading.SettingsService.Contracts.Messages
{
    public class SettingsChangedEvent
    {
        public DateTime Timestamp { get; set; }
        public SettingsTypeContract SettingsType { get; set; }
        public string Route { get; set; }
        
        /// <summary>
        /// Contain ID if a single entity was changed, null in case of batch update/insert
        /// </summary>
        [CanBeNull]
        public string ChangedEntityId { get; set; }
    }
}