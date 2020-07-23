// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class TradingRouteEntity : ITradingRoute
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string TradingConditionId { get; set; }
        public string ClientId { get; set; }
        public string Instrument { get; set; }
        OrderDirection? ITradingRoute.Type => Enum.TryParse<OrderDirection>(Type, out var parsed) ? parsed : (OrderDirection?)null; 
        public string Type { get; set; }
        public string MatchingEngineId { get; set; }
        public string Asset { get; set; }
        public string RiskSystemLimitType { get; set; }
        public string RiskSystemMetricType { get; set; }
    }
}