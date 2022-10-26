namespace MarginTrading.AssetService.Contracts.ErrorCodes
{
    public enum ClientProfilesErrorCodesContract
    {
        None,
        ClientProfileDoesNotExist,
        AssetTypeDoesNotExist,
        ClientProfileSettingsDoNotExist,
        AlreadyExist,
        CannotDeleteDefault,
        InvalidMarginValue,
        BrokerSettingsDoNotExist,
        RegulatoryTypeInMdmIsMissing,
        RegulatoryProfileInMdmIsMissing,
        RegulatorySettingsAreMissing,
        CannotSetToAvailableBecauseOfRegulatoryRestriction,
        InvalidOnBehalfFeeValue,
        InvalidExecutionFeesRate,
        InvalidExecutionFeesCap,
        InvalidExecutionFeesFloor,
        RegulationConstraintViolation,
        CannotDeleteAssetTypeAssignedToAnyProduct,
        NonDefaultUpdateForbidden,
        UnderlyingCategoryDoesNotExist
    }
}