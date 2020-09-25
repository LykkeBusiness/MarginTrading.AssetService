// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
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

        public static TradingCondition CreateFromClientProfile(ClientProfile profile, string legalEntity, decimal marginCall1, decimal marginCall2, decimal stopOut)
        {
            return new TradingCondition(
                id: profile.Id,
                name: profile.Id,
                legalEntity: legalEntity,
                marginCall1: marginCall1,
                marginCall2: marginCall2,
                stopOut: stopOut,
                depositLimit: TradingConditionConstants.DepositLimit,
                withdrawalLimit: TradingConditionConstants.WithdrawalLimit,
                limitCurrency: TradingConditionConstants.LimitCurrency,
                baseAssets: TradingConditionConstants.BaseAssets,
                isDefault: profile.IsDefault
                );
        }
    }
}