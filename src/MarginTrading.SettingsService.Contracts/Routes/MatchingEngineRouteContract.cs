// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Contracts.Enums;

namespace MarginTrading.SettingsService.Contracts.Routes
{
    public class MatchingEngineRouteContract
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string TradingConditionId { get; set; }
        public string ClientId { get; set; }
        public string Instrument { get; set; }
        public OrderDirectionContract? Type { get; set; }
        public string MatchingEngineId { get; set; }
        public string Asset { get; set; }
        public string RiskSystemLimitType { get; set; }
        public string RiskSystemMetricType { get; set; }
    }
}
