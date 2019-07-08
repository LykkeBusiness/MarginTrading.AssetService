// Copyright (c) 2019 Lykke Corp.

using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using MarginTrading.SettingsService.Core.Settings;

namespace MarginTrading.SettingsService.Extensions
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