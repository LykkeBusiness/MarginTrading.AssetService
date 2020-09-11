using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public class GetAllTickFormulasResponse
    {
        /// <summary>
        /// Collection of tick formulas
        /// </summary>
        public IReadOnlyList<TickFormulaContract> TickFormulas { get; set; }
    }
}