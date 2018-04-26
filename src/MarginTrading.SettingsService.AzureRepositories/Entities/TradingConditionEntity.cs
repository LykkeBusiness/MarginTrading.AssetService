using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class TradingConditionEntity : SimpleAzureEntity, ITradingCondition
    {
        internal override string SimplePartitionKey => "TradingConditions";
        
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