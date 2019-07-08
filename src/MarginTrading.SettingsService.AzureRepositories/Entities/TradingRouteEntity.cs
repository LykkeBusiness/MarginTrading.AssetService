// Copyright (c) 2019 Lykke Corp.

using System;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class TradingRouteEntity : SimpleAzureEntity, ITradingRoute
    {
        public static readonly string Pk = "TradingRoutes"; 
        internal override string SimplePartitionKey => Pk;
        
        // Id comes from parent type
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