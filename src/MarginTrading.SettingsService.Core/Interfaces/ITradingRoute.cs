// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface ITradingRoute
    {
        string Id { get; }
        int Rank { get; }
        string TradingConditionId { get; }
        string ClientId { get; }
        string Instrument { get; }
        OrderDirection? Type { get; }
        string MatchingEngineId { get; }
        string Asset { get; }
        string RiskSystemLimitType { get; }
        string RiskSystemMetricType { get; }
    }
}
