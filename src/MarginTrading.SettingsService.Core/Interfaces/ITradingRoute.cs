namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface ITradingRoute
    {
        string Id { get; }
        int Rank { get; }
        string TradingConditionId { get; }
        string ClientId { get; }
        string Instrument { get; }
        string Type { get; }
        string MatchingEngineId { get; }
        string Asset { get; }
        string RiskSystemLimitType { get; }
        string RiskSystemMetricType { get; }
    }
}
