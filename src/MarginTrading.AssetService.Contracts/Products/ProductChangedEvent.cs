using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.Products
{
    [MessagePackObject]
    public class ProductChangedEvent : EntityChangedEvent<ProductContract>
    {
    }
}