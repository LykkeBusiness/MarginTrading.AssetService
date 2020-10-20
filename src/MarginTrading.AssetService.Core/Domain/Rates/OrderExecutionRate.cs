// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using JetBrains.Annotations;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Settings.Rates;

namespace MarginTrading.AssetService.Core.Domain.Rates
{
    public class OrderExecutionRate
    {
        [NotNull] public string AssetPairId { get; set; }
        
        public decimal CommissionCap { get; set; }
        
        public decimal CommissionFloor { get; set; }
        
        public decimal CommissionRate { get; set; }
        
        [NotNull] public string CommissionAsset { get; set; }
        
        [CanBeNull] public string LegalEntity { get; set; }

        public static OrderExecutionRate Create(string assetPairId, decimal commissionCap, decimal commissionFloor,
            decimal commissionRate, string legalEntity)
        {
            return new OrderExecutionRate
            {
                AssetPairId = assetPairId,
                CommissionCap = commissionCap,
                CommissionFloor = commissionFloor,
                CommissionRate = commissionRate,
                LegalEntity = legalEntity,
                CommissionAsset = OrderExecutionRateConstants.CommissionAsset,
            };
        }

        public static OrderExecutionRate FromDefault(DefaultOrderExecutionSettings defaultOrderExecutionSettings,
            string assetPairId)
        {
            return new OrderExecutionRate
            {
                AssetPairId = assetPairId,
                CommissionCap = defaultOrderExecutionSettings.CommissionCap,
                CommissionFloor = defaultOrderExecutionSettings.CommissionFloor,
                CommissionRate = defaultOrderExecutionSettings.CommissionRate,
                CommissionAsset = defaultOrderExecutionSettings.CommissionAsset,
                LegalEntity = defaultOrderExecutionSettings.LegalEntity,
            };
        }
    }
}