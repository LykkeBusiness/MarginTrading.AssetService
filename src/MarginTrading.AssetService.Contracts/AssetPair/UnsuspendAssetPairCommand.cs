// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.AssetPair
{
    [Obsolete("Use ChangeProductSuspendedStatusCommand instead")]
    [MessagePackObject]
    public class UnsuspendAssetPairCommand
    {
        [Key(0)]
        public string OperationId { get; set; }
        
        [Key(1)]
        public string AssetPairId { get; set; }
    }
}