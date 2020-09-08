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
        public static async Task CreateAssetTypeAsync(HttpClient client, string regulatoryTypeId, string id, string assetTypeTemplateId = null)
        {
            var request = new AddAssetTypeRequest
            {
                RegulatoryTypeId = regulatoryTypeId,
                Username = "asdasd",
                AssetTypeTemplateId = assetTypeTemplateId,
                Id =id,
            };

            await client.PostAsync($"/api/asset-types", request.ToJsonStringContent());
        }

        public static async Task CreateClientProfileAsync(HttpClient client, string regulatoryProfileId, string id, bool isDefault, string clientProfileTemplateId = null)
        {
            var request = new AddClientProfileRequest
            {
                RegulatoryProfileId = regulatoryProfileId,
                Username = "asdasd",
                IsDefault = isDefault,
                ClientProfileTemplateId = clientProfileTemplateId,
                Id = id,
            };

            await client.PostAsync($"/api/client-profiles", request.ToJsonStringContent());
        }
    }
}