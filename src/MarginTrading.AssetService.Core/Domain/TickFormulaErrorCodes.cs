namespace MarginTrading.AssetService.Core.Domain
{
    public enum TickFormulaErrorCodes
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