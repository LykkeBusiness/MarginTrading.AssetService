namespace MarginTrading.AssetService.Contracts.TickFormula
{
    public enum TickFormulaErrorCodesContract
    {
        None,
        AlreadyExist,
        TickFormulaDoesNotExist,
        PdlLaddersAndTicksMustHaveEqualLengths,
        PdlLaddersMustStartFromZero,
        PdlLaddersMustBeInAscendingOrderWithoutDuplicates,
        PdlTicksMustBeInAscendingOrder,
        PdlLaddersValuesMustBeGreaterOrEqualToZero,
        PdlTicksValuesMustBeGreaterThanZero,
        CannotDeleteTickFormulaAssignedToAnyProduct
    }
}