// Copyright (c) 2019 Lykke Corp.

using MessagePack;

namespace MarginTrading.SettingsService.Contracts.AssetPair
{
    [MessagePackObject]
    public class UnsuspendAssetPairCommand
    {
        [Key(0)]
        public string OperationId { get; set; }
        
        [Key(1)]
        public string AssetPairId { get; set; }
    }
}