using System.Collections.Generic;

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface ITradingCondition
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
