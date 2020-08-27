using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Tests.Extensions;

namespace MarginTrading.AssetService.Tests.Common
{
    public class TestRecordsCreator
    {
        public static async Task<Guid> CreateAssetTypeAsync(HttpClient client, Guid regulatoryTypeId, string name, Guid? assetTypeTemplateId = null)
        {
            var request = new AddAssetTypeRequest
            {
                RegulatoryTypeId = regulatoryTypeId,
                Username = "asdasd",
                Name = name,
                AssetTypeTemplateId = assetTypeTemplateId,
            };

            var res = await client.PostAsync($"/api/asset-types", request.ToJsonStringContent());

            var getAssetTypesRequest = await client.GetAsync("/api/asset-types");
            var assetTypeId = (await getAssetTypesRequest.Content.ReadAsStringAsync())
                .DeserializeJson<GetAllAssetTypesResponse>().AssetTypes.First(x => x.Name == name).Id;

            return assetTypeId;
        }

        public static async Task<Guid> CreateClientProfileAsync(HttpClient client, Guid regulatoryProfileId , string name, bool isDefault, Guid? clientProfileTemplateId = null)
        {
            var request = new AddClientProfileRequest
            {
                RegulatoryProfileId = regulatoryProfileId,
                Username = "asdasd",
                Name = name,
                IsDefault = isDefault,
                ClientProfileTemplateId = clientProfileTemplateId,
            };

            var res = await client.PostAsync($"/api/client-profiles", request.ToJsonStringContent());

            var getClientProfilesRequest = await client.GetAsync("/api/client-profiles");
            var clientProfileId = (await getClientProfilesRequest.Content.ReadAsStringAsync())
                .DeserializeJson<GetAllClientProfilesResponse>().ClientProfiles.First(x => x.Name == name).Id;

            return clientProfileId;
        }
    }
}