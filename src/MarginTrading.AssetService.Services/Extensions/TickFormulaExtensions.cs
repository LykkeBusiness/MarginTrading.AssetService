using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class TickFormulaExtensions
    {
        public static TickFormula ToDomainModel(this ITickFormula src) =>
            new TickFormula
            {
                Id = src.Id,
                PdlLadders = src.PdlLadders,
                PdlTicks = src.PdlTicks
            };
    }
}