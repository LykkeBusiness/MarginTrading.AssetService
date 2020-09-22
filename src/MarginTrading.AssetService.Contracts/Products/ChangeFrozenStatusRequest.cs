using System;
using MarginTrading.AssetService.Contracts.Core;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class ChangeFrozenStatusRequest : UserRequest
    {
        public bool IsFrozen { get; set; }
        public ProductFreezeInfoContract FreezeInfo { get; set; }
    }
}