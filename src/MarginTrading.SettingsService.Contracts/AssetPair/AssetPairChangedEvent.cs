// Copyright (c) 2019 Lykke Corp.

using JetBrains.Annotations;
using MessagePack;

namespace MarginTrading.SettingsService.Contracts.AssetPair
{
    [MessagePackObject]
    public class AssetPairChangedEvent
    {
        [CanBeNull]
        [Key(0)]
        public string OperationId { get; set; }
        
        [NotNull]
        [Key(1)]
        public AssetPairContract AssetPair { get; set; }
    }
}