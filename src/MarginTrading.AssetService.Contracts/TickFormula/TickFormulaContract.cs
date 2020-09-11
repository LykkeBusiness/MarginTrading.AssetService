using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public class TickFormulaContract
    {
        /// <summary>
        /// id of the tick formula
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// PdlTicks ladders
        /// </summary>
        public List<decimal> PdlLadders { get; set; }

        /// <summary>
        /// Pdl ticks
        /// </summary>
        public List<decimal> PdlTicks { get; set; }
    }
}