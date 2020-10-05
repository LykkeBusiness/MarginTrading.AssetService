using MarginTrading.AssetService.Contracts.Common;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    [MessagePackObject]
    public class ClientProfileChangedEvent : EntityChangedEvent<ClientProfileContract>
    {
    }
}