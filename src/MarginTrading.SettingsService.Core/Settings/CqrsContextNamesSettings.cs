using Lykke.SettingsReader.Attributes;

namespace MarginTrading.SettingsService.Core.Settings
{
    public class CqrsContextNamesSettings
    {
        [Optional] public string SettingsService { get; set; } = nameof(SettingsService);

        [Optional] public string TradingEngine { get; set; } = nameof(TradingEngine);
    }
}