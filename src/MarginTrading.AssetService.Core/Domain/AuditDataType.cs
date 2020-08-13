using System.ComponentModel;

namespace MarginTrading.AssetService.Core.Domain
{
    public enum AuditDataType
    {
        [Description("Broker regualtory profile")]
        BrokerRegulatoryProfile,
        [Description("Broker regulatory type")]
        BrokerRegulatoryType,
        [Description("Broker regulatory settings")]
        BrokerRegulatorySettings
    }
}