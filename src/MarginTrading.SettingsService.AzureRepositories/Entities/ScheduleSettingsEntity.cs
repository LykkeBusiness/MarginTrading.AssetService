using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class ScheduleSettingsEntity : SimpleAzureEntity, IScheduleSettingsEntity
    {
        public ScheduleSettingsEntity()
        {
            PartitionKey = "ScheduleSettings";
        }
        
        // Id comes from parent type
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        public string AssetPairs { get; set; }
        public string MarketId { get; set; }
        public bool? IsTradeEnabled { get; set; }
        public string PendingOrdersCutOff { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}