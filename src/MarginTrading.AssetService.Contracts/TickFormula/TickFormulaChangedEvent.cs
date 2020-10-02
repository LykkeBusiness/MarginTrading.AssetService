using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    [MessagePackObject]
    public class TickFormulaChangedEvent : EntityChangedEvent<TickFormulaContract>
    {
    }
}