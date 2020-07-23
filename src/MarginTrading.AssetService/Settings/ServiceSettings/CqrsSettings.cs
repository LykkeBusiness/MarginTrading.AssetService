// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.Settings.ServiceSettings
{
    public class CqrsSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
 
        public TimeSpan RetryDelay { get; set; }
 
        [Optional, CanBeNull]
        public string EnvironmentName { get; set; }
 
        [Optional]
        public CqrsContextNamesSettings ContextNames { get; set; } = new CqrsContextNamesSettings();
    }
}