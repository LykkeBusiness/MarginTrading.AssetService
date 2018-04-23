namespace MarginTrading.SettingsService.StorageInterfaces.Entities
{
    public interface ITradingConditionEntity
    {
        string Id { get; }
        string Name { get; }
        string LegalEntity { get; }
        decimal MarginCall1 { get; }
        decimal MarginCall2 { get; }
        decimal StopOut { get; }
        decimal DepositLimit { get; }
        decimal WithdrawalLimit { get; }
        string LimitCurrency { get; }
        string BaseAssets { get; }
    }
}
