using System.ComponentModel;

namespace MarginTrading.AssetService.Core.Domain
{
    public enum AuditDataType
    {
        [Description("Client profile")]
        ClientProfile,
        [Description("Asset type")]
        AssetType,
        [Description("Client profile settings")]
        ClientProfileSettings
    }
}