// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Domain
{
    public enum FreezeReason
    {
        Undefined = 0,
        
        CorporateAction = 1,
        
        Manual = 2,
        
        CostAndChargesGeneration = 3
    }
}