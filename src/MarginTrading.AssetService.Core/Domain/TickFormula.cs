using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Domain
{
    public class TickFormula : ITickFormula
    {
        public string Id { get; set; }

        public List<decimal> PdlLadders { get; set; }

        public List<decimal> PdlTicks { get; set; }
    }
}