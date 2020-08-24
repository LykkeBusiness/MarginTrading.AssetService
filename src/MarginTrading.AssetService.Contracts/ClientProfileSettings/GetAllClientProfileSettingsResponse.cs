using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class GetAllClientProfileSettingsResponse
    {
        public IReadOnlyList<ClientProfileSettingsContract> ClientProfileSettings { get; set; }
    }
}