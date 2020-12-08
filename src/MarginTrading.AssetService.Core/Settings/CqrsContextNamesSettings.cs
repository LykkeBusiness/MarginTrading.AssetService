// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.SettingsReader.Attributes;

namespace MarginTrading.AssetService.Core.Settings
{
    public class CqrsContextNamesSettings
    {
        [Optional] public string AssetService { get; set; } = "SettingsService";

        [Optional] public string TradingEngine { get; set; } = nameof(TradingEngine);

        [Optional] public string MdmService { get; set; } = "MdmService";
        
        [Optional] public string BookKeeper { get; set; } = nameof(BookKeeper);

    }
}