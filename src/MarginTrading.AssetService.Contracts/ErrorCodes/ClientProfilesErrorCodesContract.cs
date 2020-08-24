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
        InvalidMarginMinValue,
        BrokerSettingsDoNotExist,
        RegulatoryTypeInMdmIsMissing,
        RegulatoryProfileInMdmIsMissing,
        RegulatorySettingsAreMissing,
        CannotSetToAvailableBecauseOfRegulatoryRestriction,
        InvalidPhoneFeesValue,
        InvalidExecutionFeesRate,
        InvalidExecutionFeesCap,
        InvalidExecutionFeesFloor,
    }
}