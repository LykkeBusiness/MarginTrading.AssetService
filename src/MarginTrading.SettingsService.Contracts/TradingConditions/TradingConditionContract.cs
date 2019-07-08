﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MarginTrading.SettingsService.Contracts.TradingConditions
{
    public class TradingConditionContract
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LegalEntity { get; set; }
        public decimal MarginCall1 { get; set; }
        public decimal MarginCall2 { get; set; }
        public decimal StopOut { get; set; }
        public decimal DepositLimit { get; set; }
        public decimal WithdrawalLimit { get; set; }
        public string LimitCurrency { get; set; }
        public List<string> BaseAssets { get; set; }
        public bool IsDefault { get; set; }
    }
}
