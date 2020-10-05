using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    [MessagePackObject]
    public class AssetTypeChangedEvent : EntityChangedEvent<AssetTypeContract>
    {
    }
}