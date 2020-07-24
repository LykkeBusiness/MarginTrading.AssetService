// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.AssetPair
{
    [MessagePackObject]
    public class FreezeInfoContract
    {
        [Key(0)]
        public FreezeReasonContract Reason { get; set; }
        
        [Key(1)]
        public string Comment { get; set; }
        
        [Key(2)]
        public DateTime? UnfreezeDate { get; set; }
    }
}