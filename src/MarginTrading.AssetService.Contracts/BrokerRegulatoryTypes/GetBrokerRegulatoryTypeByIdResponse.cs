using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.ErrorCodes;

namespace MarginTrading.AssetService.Contracts.BrokerRegulatoryTypes
{
    public class GetBrokerRegulatoryTypeByIdResponse : ErrorCodeResponse<BrokerRegulationsErrorCodesContract>
    {
        public BrokerRegulatoryTypeContract BrokerRegulatoryType { get; set; }
    }
}