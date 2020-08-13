using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatorySettings
{
    public class GetBrokerRegulatorySettingsByIdsResponse : ErrorCodeResponse<BrokerRegulationsErrorCodesContract>
    {
        public BrokerRegulatorySettingsContract BrokerRegulatorySettings { get; set; }
    }
}