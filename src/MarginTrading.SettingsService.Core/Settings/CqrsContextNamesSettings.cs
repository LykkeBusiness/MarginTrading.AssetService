// Copyright (c) 2019 Lykke Corp.

using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Core.Settings
{
    public class CqrsContextNamesSettings
    {
        [Optional] public string SettingsService { get; set; } = nameof(SettingsService);

        [Optional] public string TradingEngine { get; set; } = nameof(TradingEngine);
    }
}