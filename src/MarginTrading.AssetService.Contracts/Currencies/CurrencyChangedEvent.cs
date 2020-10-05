using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    [MessagePackObject]
    public class CurrencyChangedEvent : EntityChangedEvent<CurrencyContract>
    {
    }
}