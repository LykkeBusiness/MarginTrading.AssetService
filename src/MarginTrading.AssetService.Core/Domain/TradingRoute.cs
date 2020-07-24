// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class TradingRoute : ITradingRoute
    {
        public TradingRoute(string id, int rank, string tradingConditionId, string clientId, string instrument, 
            OrderDirection? type, string matchingEngineId, string asset, string riskSystemLimitType, string riskSystemMetricType)
        {
            Id = id;
            Rank = rank;
            TradingConditionId = tradingConditionId;
            ClientId = clientId;
            Instrument = instrument;
            Type = type;
            MatchingEngineId = matchingEngineId;
            Asset = asset;
            RiskSystemLimitType = riskSystemLimitType;
            RiskSystemMetricType = riskSystemMetricType;
        }

        public string Id { get; }
        public int Rank { get; }
        public string TradingConditionId { get; }
        public string ClientId { get; }
        public string Instrument { get; }
        public OrderDirection? Type { get; }
        public string MatchingEngineId { get; }
        public string Asset { get; }
        public string RiskSystemLimitType { get; }
        public string RiskSystemMetricType { get; }
    }
}