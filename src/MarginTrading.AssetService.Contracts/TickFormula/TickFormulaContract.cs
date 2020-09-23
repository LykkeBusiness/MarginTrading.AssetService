using System.Collections.Generic;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    [MessagePackObject]
    public class TickFormulaContract
    {
        /// <summary>
        /// id of the tick formula
        /// </summary>
        [Key(0)]
        public string Id { get; set; }

        /// <summary>
        /// PdlTicks ladders
        /// </summary>
        [Key(1)]
        public List<decimal> PdlLadders { get; set; }

        /// <summary>
        /// Pdl ticks
        /// </summary>
        [Key(2)]
        public List<decimal> PdlTicks { get; set; }
    }
}