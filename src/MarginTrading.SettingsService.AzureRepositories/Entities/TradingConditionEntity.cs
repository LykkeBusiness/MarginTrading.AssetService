using MarginTrading.SettingsService.StorageInterfaces.Entities;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class TradingConditionEntity : SimpleAzureEntity, ITradingConditionEntity
    {
        public TradingConditionEntity()
        {
            PartitionKey = "TradingConditions";
        }
        
        // Id comes from parent type
        public string Name { get; set; }
        public string LegalEntity { get; set; }
        public decimal MarginCall1 { get; set; }
        public decimal MarginCall2 { get; set; }
        public decimal StopOut { get; set; }
        public decimal DepositLimit { get; set; }
        public decimal WithdrawalLimit { get; set; }
        public string LimitCurrency { get; set; }
        public string BaseAssets { get; set; }
    }
}