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
        [MaxLength(100)]
        [RegularExpression("^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "Id must contain only alphanumeric values and underscores and must start with a letter.")]
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