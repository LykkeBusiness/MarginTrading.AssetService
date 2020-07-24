// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.TradingConditions;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.Extensions
{
    public static class LegalEntityExtensions
    {

        public static void Set(this DefaultLegalEntitySettings defaults, AssetPairContract contract)
        {
            if (string.IsNullOrWhiteSpace(contract.LegalEntity))
            {
                contract.LegalEntity = defaults.DefaultLegalEntity;
            }
        }
            
        public static void Set(this DefaultLegalEntitySettings defaults, TradingConditionContract contract)
        {
            if (string.IsNullOrWhiteSpace(contract.LegalEntity))
            {
                contract.LegalEntity = defaults.DefaultLegalEntity;
            }
        }
    }
}