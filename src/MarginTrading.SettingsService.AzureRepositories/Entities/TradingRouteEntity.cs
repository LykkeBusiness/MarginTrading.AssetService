using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class TradingRouteEntity : SimpleAzureEntity, ITradingRoute
    {
        internal override string SimplePartitionKey => "TradingRoutes";
        
        // Id comes from parent type
        public int Rank { get; set; }
        public string TradingConditionId { get; set; }
        public string ClientId { get; set; }
        public string Instrument { get; set; }
        public string Type { get; set; }
        public string MatchingEngineId { get; set; }
        public string Asset { get; set; }
        public string RiskSystemLimitType { get; set; }
        public string RiskSystemMetricType { get; set; }
    }
}