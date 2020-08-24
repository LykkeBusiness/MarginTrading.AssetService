using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    public class GetAllClientProfilesResponse
    {
        public IReadOnlyList<ClientProfileContract> ClientProfiles { get; set; }
    }
}