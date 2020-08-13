namespace MarginTrading.AssetService.Contracts.ErrorCodes
{
    public enum BrokerRegulationsErrorCodesContract
    {
        None,
        BrokerRegulatoryProfileDoesNotExist,
        BrokerRegulatoryTypeDoesNotExist,
        BrokerRegulatorySettingsDoNotExist,
        AlreadyExist,
        CannotDeleteDefault,
        InvalidMarginMinValue,
    }
}