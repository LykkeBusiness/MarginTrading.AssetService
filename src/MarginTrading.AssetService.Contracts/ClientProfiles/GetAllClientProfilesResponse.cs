using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    /// <summary>
    /// Response model to get all client profiles
    /// </summary>
    public class GetAllClientProfilesResponse
    {
        /// <summary>
        /// Collection of client profiles
        /// </summary>
        public IReadOnlyList<ClientProfileContract> ClientProfiles { get; set; }
    }
}