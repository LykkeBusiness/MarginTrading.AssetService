using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.SettingsService.Core.Domain
{
    public class TradingCondition : ITradingCondition
    {
        public TradingCondition(string id, string name, string legalEntity, decimal marginCall1, decimal marginCall2, 
            decimal stopOut, decimal depositLimit, decimal withdrawalLimit, string limitCurrency, 
            List<string> baseAssets, bool isDefault)
        {
            Id = id;
            Name = name;
            LegalEntity = legalEntity;
            MarginCall1 = marginCall1;
            MarginCall2 = marginCall2;
            StopOut = stopOut;
            DepositLimit = depositLimit;
            WithdrawalLimit = withdrawalLimit;
            LimitCurrency = limitCurrency;
            BaseAssets = baseAssets;
            IsDefault = isDefault;
        }

        public string Id { get; }
        public string Name { get; }
        public string LegalEntity { get; }
        public decimal MarginCall1 { get; }
        public decimal MarginCall2 { get; }
        public decimal StopOut { get; }
        public decimal DepositLimit { get; }
        public decimal WithdrawalLimit { get; }
        public string LimitCurrency { get; }
        public List<string> BaseAssets { get; }
        public bool IsDefault { get; set; }
    }
}