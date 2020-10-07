using Lykke.SettingsReader.Attributes;

namespace MarginTrading.AssetService.Core.Settings
{
    public class DefaultTradingConditionsSettings
    {
        [Optional] 
        public decimal MarginCall1 { get; set; } = 1.25M;

        [Optional] 
        public decimal MarginCall2 { get; set; } = 1.11M;

        [Optional] 
        public decimal StopOut { get; set; } = 1;
    }
}