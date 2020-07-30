﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Interfaces
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
        List<string> BaseAssets { get; }
        bool IsDefault { get; }
    }
}
