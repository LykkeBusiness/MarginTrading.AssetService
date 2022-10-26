using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Tests.Common;
using MarginTrading.AssetService.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests.FeatureTests
{
    public class ClientProfileSettingsFeature
    {
        private readonly Mock<IBrokerSettingsApi> _brokerSettingsApiMock = new Mock<IBrokerSettingsApi>();
        private readonly Mock<IRegulationsApi> _regulationsApiMock = new Mock<IRegulationsApi>();
        private readonly Mock<IRegulatoryTypesApi> _regulatoryTypesApiMock = new Mock<IRegulatoryTypesApi>();
        private readonly Mock<IRegulatorySettingsApi> _regulatorySettingsApiMock = new Mock<IRegulatorySettingsApi>();
        private readonly Mock<IRegulatoryProfilesApi> _regulatoryProfilesApiMock = new Mock<IRegulatoryProfilesApi>();
        private readonly Mock<IUnderlyingCategoriesCache> _underlyingCategoriesCacheMock = new Mock<IUnderlyingCategoriesCache>();
        private readonly Mock<IUnderlyingCategoriesApi> _underlyingCategoriesApiMock = new Mock<IUnderlyingCategoriesApi>();

        private const string RegulationId = "03041938-177b-4178-928d-3d3696cfae15";
        private const string RegulatoryTypeId = "03041938-177b-4178-928d-3d3696cfae22";
        private const string RegulatoryProfileId = "12341938-177b-4178-928d-3d3696cfae22";
        private const string AssetTypeId = "asset-1";
        private const string ClientProfileId = "Default";
        private const string SecondAssetTypeId = "asset-2";
        private const string SecondClientProfileId = "client-2";
        private const string UnderlyingCategoryId = "UnderlyingCategoryId";
        private const decimal MarginRate = 60M;
        private const decimal MarginMinPercent = 50;

        [Fact]
        public async Task TestClientProfileSettingsWorkflow()
        {

            _brokerSettingsApiMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetBrokerSettingsByIdResponse
                {
                    ErrorCode = BrokerSettingsErrorCodesContract.None,
                    BrokerSettings = new BrokerSettingsContract {RegulationId = RegulationId}
                });

            _regulatoryTypesApiMock.Setup(x => x.GetRegulatoryTypeByIdAsync(RegulatoryTypeId))
                .ReturnsAsync(new GetRegulatoryTypeByIdResponse
                {
                    RegulatoryType = new RegulatoryTypeContract
                    {
                        RegulationId = RegulationId
                    }
                });

            _regulatoryProfilesApiMock.Setup(x => x.GetRegulatoryProfileByIdAsync(RegulatoryProfileId))
                .ReturnsAsync(new GetRegulatoryProfileByIdResponse
                {
                    RegulatoryProfile = new RegulatoryProfileContract
                    {
                        RegulationId = RegulationId
                    }
                });

            _regulatorySettingsApiMock.Setup(x => x.GetRegulatorySettingsByIdsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new GetRegulatorySettingsByIdsResponse
                {
                    RegulatorySettings = new RegulatorySettingsContract
                    {
                        IsAvailable = true,
                        MarginMinPercent = MarginMinPercent
                    }
                });

            _regulatorySettingsApiMock.Setup(x => x.GetRegulatorySettingsByRegulationAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetRegulatorySettingsResponse
                {
                    RegulatorySettings = new List<RegulatorySettingsContract>
                    {
                        new RegulatorySettingsContract
                        {
                            MarginMinPercent = MarginMinPercent,
                            IsAvailable = true,
                            TypeId = RegulatoryTypeId,
                            ProfileId = RegulatoryProfileId
                        }
                    }
                });

            _underlyingCategoriesCacheMock.Setup(x => x.Get()).ReturnsAsync(new List<UnderlyingCategoryCacheModel>
            {
                new UnderlyingCategoryCacheModel
                {
                    Id = UnderlyingCategoryId,
                    FinancingFeesFormula = ""
                }
            });

            _underlyingCategoriesApiMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new GetUnderlyingCategoriesResponse
            {
                UnderlyingCategories = new List<UnderlyingCategoryContract>
                {
                    new UnderlyingCategoryContract
                    {
                        Id = UnderlyingCategoryId,
                        FinancingFeesFormula = ""
                    }
                }
            });

            var client = TestBootstrapper.CreateTestClientWithInMemoryDb(services =>
            {
                services.AddSingleton(_brokerSettingsApiMock.Object);
                services.AddSingleton(_regulationsApiMock.Object);
                services.AddSingleton(_regulatoryTypesApiMock.Object);
                services.AddSingleton(_regulatorySettingsApiMock.Object);
                services.AddSingleton(_regulatoryProfilesApiMock.Object);
                services.AddSingleton(_underlyingCategoriesApiMock.Object);
                services.AddSingleton(_underlyingCategoriesCacheMock.Object);
            });

            var createClientProfileResponse = await TestRecordsCreator.CreateClientProfileAsync(client, RegulatoryProfileId, ClientProfileId, true);
            Assert.Equal(ClientProfilesErrorCodesContract.None, createClientProfileResponse.ErrorCode);

            var createAssetTypeResponse = await TestRecordsCreator.CreateAssetTypeAsync(client, RegulatoryTypeId, AssetTypeId, UnderlyingCategoryId);
            Assert.Equal(ClientProfilesErrorCodesContract.None, createAssetTypeResponse.ErrorCode);

            var updateClientProfileSettingsRequest = new UpdateClientProfileSettingsRequest
            {
                Margin = MarginRate,
                IsAvailable = true,
                Username = "asd"
            };

            await client.PutAsync($"/api/client-profile-settings/profile/{ClientProfileId}/type/{AssetTypeId}",
                updateClientProfileSettingsRequest.ToJsonStringContent());

            var createAssetTypeResponse2 = await TestRecordsCreator.CreateAssetTypeAsync(client, RegulatoryTypeId, SecondAssetTypeId, UnderlyingCategoryId, AssetTypeId);
            Assert.Equal(ClientProfilesErrorCodesContract.None, createAssetTypeResponse2.ErrorCode);

            var createClientProfileResponse2 = await TestRecordsCreator.CreateClientProfileAsync(client, RegulatoryProfileId, SecondClientProfileId, false, ClientProfileId);
            Assert.Equal(ClientProfilesErrorCodesContract.None, createClientProfileResponse2.ErrorCode);

            //Get all client profile settings for this regulation
            var getClientProfileSettingsRequest = await client.GetAsync($"/api/client-profile-settings");
            var clientProfileSettings = (await getClientProfileSettingsRequest.Content.ReadAsStringAsync())
                .DeserializeJson<GetAllClientProfileSettingsResponse>().ClientProfileSettings;

            //Check if the result contains settings with copied values from the templates
            var containsSettingsCreatedFromProfileTemplate = clientProfileSettings.Any(s =>
                s.IsAvailable && s.AssetTypeId == AssetTypeId && s.ClientProfileId == SecondClientProfileId &&
                s.Margin == MarginRate);

            var containsSettingsCreatedFromTypeTemplate = clientProfileSettings.Any(s =>
                s.IsAvailable && s.AssetTypeId == SecondAssetTypeId && s.ClientProfileId == ClientProfileId &&
                s.Margin == MarginRate);

            Assert.True(containsSettingsCreatedFromProfileTemplate);
            Assert.True(containsSettingsCreatedFromTypeTemplate);
            Assert.Equal(4, clientProfileSettings.Count);
        }

        [Fact]
        public void NonDefaultProfileIdCannotBeSetAsDefault()
        {
            Assert.Throws<ClientProfileNonDefaultUpdateForbiddenException>(() =>
                new ClientProfile(SecondClientProfileId, RegulatoryTypeId, true));
        }
    }
}