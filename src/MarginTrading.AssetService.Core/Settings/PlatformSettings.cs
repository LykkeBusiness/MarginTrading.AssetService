// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.SettingsReader.Attributes;

namespace MarginTrading.AssetService.Core.Settings
{
    public class PlatformSettings
    {
        [Optional]
        public string PlatformMarketId { get; set; } = "PlatformScheduleMarketId";
    }
}