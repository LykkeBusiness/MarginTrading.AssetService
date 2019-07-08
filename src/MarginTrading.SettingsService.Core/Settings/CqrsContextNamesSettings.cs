// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Core.Settings
{
    public class CqrsContextNamesSettings
    {
        [Optional] public string SettingsService { get; set; } = nameof(SettingsService);

        [Optional] public string TradingEngine { get; set; } = nameof(TradingEngine);
    }
}