using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Tests.Common;
using MarginTrading.AssetService.Tests.Extensions;
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

        private const string RegulationId = "03041938-177b-4178-928d-3d3696cfae15";
        private const string RegulatoryTypeId = "03041938-177b-4178-928d-3d3696cfae22";
        private const string RegulatoryProfileId = "12341938-177b-4178-928d-3d3696cfae22";
        private const string AssetTypeId = "asset-1";
        private const string ClientProfileId = "client-1";
        private const string SecondAssetTypeId = "asset-2";
        private const string SecondClientProfileId = "client-2";
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
                .ReturnsAsync(new GetRegulatorySettingsByIdsResponse()
                {
                    RegulatorySettings = new RegulatorySettingsContract()
                    {
                        IsAvailable = true,
                        MarginMinPercent = MarginMinPercent,
                    }
                });

            _regulatorySettingsApiMock.Setup(x => x.GetRegulatorySettingsByRegulationAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetRegulatorySettingsResponse()
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

            var client = await TestBootstrapper.CreateTestClientWithInMemoryDb(builder =>
            {
                builder.RegisterInstance(_brokerSettingsApiMock.Object).As<IBrokerSettingsApi>().SingleInstance();
                builder.RegisterInstance(_regulationsApiMock.Object).As<IRegulationsApi>().SingleInstance();
                builder.RegisterInstance(_regulatoryTypesApiMock.Object).As<IRegulatoryTypesApi>().SingleInstance();
                builder.RegisterInstance(_regulatorySettingsApiMock.Object).As<IRegulatorySettingsApi>().SingleInstance();
                builder.RegisterInstance(_regulatoryProfilesApiMock.Object).As<IRegulatoryProfilesApi>().SingleInstance();
            });

            await TestRecordsCreator.CreateAssetTypeAsync(client, RegulatoryTypeId, AssetTypeId);

            await TestRecordsCreator.CreateClientProfileAsync(client, RegulatoryProfileId, ClientProfileId, true);

            var updateClientProfileSettingsRequest = new UpdateClientProfileSettingsRequest
            {
                Margin = MarginRate,
                IsAvailable = true,
                Username = "asd"
            };

            await client.PutAsync($"/api/client-profile-settings/profile/{ClientProfileId}/type/{AssetTypeId}",
                updateClientProfileSettingsRequest.ToJsonStringContent());

            await TestRecordsCreator.CreateAssetTypeAsync(client, RegulatoryTypeId, SecondAssetTypeId, AssetTypeId);

            await TestRecordsCreator.CreateClientProfileAsync(client, RegulatoryProfileId, SecondClientProfileId, true, ClientProfileId);

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
    }
}