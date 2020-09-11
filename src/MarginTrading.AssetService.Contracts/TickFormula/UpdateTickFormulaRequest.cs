using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public class UpdateTickFormulaRequest
    {
        /// <summary>
        /// PdlTicks ladders
        /// </summary>
        [Required]
        public List<decimal> PdlLadders { get; set; }

        /// <summary>
        /// Pdl ticks
        /// </summary>
        [Required]
        public List<decimal> PdlTicks { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }
    }
}