using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public class AddTickFormulaRequest
    {
        /// <summary>
        /// id of the tick formula
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// PdlTicks ladders
        /// </summary>
        public List<decimal> PdlLadders { get; set; }

        /// <summary>
        /// Pdl ticks
        /// </summary>
        public List<decimal> PdlTicks { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }
    }
}