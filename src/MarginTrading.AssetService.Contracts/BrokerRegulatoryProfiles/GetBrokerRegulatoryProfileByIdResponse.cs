using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryProfiles
{
    public class GetBrokerRegulatoryProfileByIdResponse : ErrorCodeResponse<BrokerRegulationsErrorCodesContract>
    {
        public BrokerRegulatoryProfileContract BrokerRegulatoryProfile { get; set; }
    }
}