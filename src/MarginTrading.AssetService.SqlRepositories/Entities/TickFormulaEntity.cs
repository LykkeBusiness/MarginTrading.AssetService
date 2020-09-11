using System.Collections.Generic;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class TickFormulaEntity : ITickFormula
    {
        public string Id { get; set; }
        public List<decimal> PdlLadders { get; set; }
        public List<decimal> PdlTicks { get; set; }

        public static TickFormulaEntity Create(ITickFormula model)
        {
            return new TickFormulaEntity
            {
                Id = model.Id,
                PdlLadders = model.PdlLadders,
                PdlTicks = model.PdlTicks,
            };
        }
    }
}