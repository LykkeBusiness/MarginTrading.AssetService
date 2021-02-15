// Copyright (c) 2019 Lykke Corp.
//         See the LICENSE file in the project root for more information.

using System;

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    [Serializable]
    public class ClientProfile
    {
        public string Id { get; set; }
        
        public decimal Margin { get; set; }
        
        public decimal ExecutionFeesFloor { get; set; }
        
        public decimal ExecutionFeesCap { get; set; }
        
        public decimal ExecutionFeesRate { get; set; }
        
        public decimal FinancingFeesRate { get; set; }
        
        public decimal OnBehalfFee { get; set; }
    }
}