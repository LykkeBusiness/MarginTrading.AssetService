// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;
using MarginTrading.AssetService.Core.Settings.Rates;

namespace MarginTrading.AssetService.Core.Domain.Rates
{
    public class OvernightSwapRate
    {
        [NotNull] public string AssetPairId { get; }
        
        public decimal RepoSurchargePercent { get; }
        
        [CanBeNull] public string VariableRateBase { get; }
        
        [CanBeNull] public string VariableRateQuote { get; }

        public OvernightSwapRate(string assetPairId, decimal repoSurchargePercent, string variableRateBase, string variableRateQuote)
        {
            if (string.IsNullOrEmpty(assetPairId))
                throw new ArgumentNullException(nameof(assetPairId), "Value cannot be null or empty");
            
            AssetPairId = assetPairId;
            RepoSurchargePercent = repoSurchargePercent;
            VariableRateBase = variableRateBase;
            VariableRateQuote = variableRateQuote;
        }

        public static OvernightSwapRate FromDefault(DefaultOvernightSwapSettings defaultOvernightSwapSettings,
            string assetPairId)
        {
            return new OvernightSwapRate(
                assetPairId,
                defaultOvernightSwapSettings.RepoSurchargePercent,
                defaultOvernightSwapSettings.VariableRateBase,
                defaultOvernightSwapSettings.VariableRateQuote);
        }
    }
}