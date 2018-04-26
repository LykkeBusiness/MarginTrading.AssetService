namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IScheduleSettings
    {
        string Id { get; }
        int Rank { get; }
        string AssetPairRegex { get; }
        string AssetPairs { get; }
        string MarketId { get; }

        bool? IsTradeEnabled { get; }
        string PendingOrdersCutOff { get; }

        string Start { get; }
        string End { get; }
   }
}
