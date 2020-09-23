using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    [MessagePackObject]
    public class ClientProfileSettingsChangedEvent : EntityChangedEvent<ClientProfileSettingsContract>
    {
    }
}