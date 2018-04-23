namespace MarginTrading.SettingsService.StorageInterfaces.Entities
{
    public interface IScheduleSettingsEntity
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
