namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public class GetTickFormulaByIdResponse
    {
        /// <summary>
        /// Tick formula contract
        /// </summary>
        public TickFormulaContract TickFormula { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public TickFormulaErrorCodesContract Error { get; set; }
    }
}