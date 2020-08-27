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
        private const string AssetTypeName = "asset-1";
        private const string ClientProfileName = "client-1";
        private const string SecondAssetTypeName = "asset-2";
        private const string SecondClientProfileName = "client-2";
        private const decimal MarginRate = 0.6M;
        private const int MarginMinPercent = 50;

        [Fact]
        public async Task TestClientProfileSettingsWorkflow()
        {

            _brokerSettingsApiMock.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetBrokerSettingsByIdResponse
                {
                    ErrorCode = BrokerSettingsErrorCodesContract.None,
                    BrokerSettings = new BrokerSettingsContract {RegulationId = Guid.Parse(RegulationId)}
                });

            _regulatoryTypesApiMock.Setup(x => x.GetRegulatoryTypeByIdAsync(Guid.Parse(RegulatoryTypeId)))
                .ReturnsAsync(new GetRegulatoryTypeByIdResponse
                {
                    RegulatoryType = new RegulatoryTypeContract
                    {
                        RegulationId = Guid.Parse(RegulationId)
                    }
                });

            _regulatoryProfilesApiMock.Setup(x => x.GetRegulatoryProfileByIdAsync(Guid.Parse(RegulatoryProfileId)))
                .ReturnsAsync(new GetRegulatoryProfileByIdResponse
                {
                    RegulatoryProfile = new RegulatoryProfileContract
                    {
                        RegulationId = Guid.Parse(RegulationId)
                    }
                });

            _regulatorySettingsApiMock.Setup(x => x.GetRegulatorySettingsByIdsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new GetRegulatorySettingsByIdsResponse()
                {
                    RegulatorySettings = new RegulatorySettingsContract()
                    {
                        IsAvailable = true,
                        MarginMinPercent = MarginMinPercent,
                    }
                });

            _regulatorySettingsApiMock.Setup(x => x.GetRegulatorySettingsByRegulationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetRegulatorySettingsResponse()
                {
                    RegulatorySettings = new List<RegulatorySettingsContract>
                    {
                        new RegulatorySettingsContract
                        {
                            MarginMinPercent = MarginMinPercent,
                            IsAvailable = true,
                            TypeId = Guid.Parse(RegulatoryTypeId),
                            ProfileId = Guid.Parse(RegulatoryProfileId)
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

            var assetId = await TestRecordsCreator.CreateAssetTypeAsync(client, Guid.Parse(RegulatoryTypeId), AssetTypeName);

            var clientProfileId = await TestRecordsCreator.CreateClientProfileAsync(client, Guid.Parse(RegulatoryProfileId), ClientProfileName, true);

            var updateClientProfileSettingsRequest = new UpdateClientProfileSettingsRequest
            {
                Margin = MarginRate,
                IsAvailable = true,
                Username = "asd"
            };

            await client.PutAsync($"/api/client-profile-settings/profile/{clientProfileId}/type/{assetId}",
                updateClientProfileSettingsRequest.ToJsonStringContent());

            var assetI2d = await TestRecordsCreator.CreateAssetTypeAsync(client, Guid.Parse(RegulatoryTypeId), SecondAssetTypeName, assetId);

            var clientProfileId2 = await TestRecordsCreator.CreateClientProfileAsync(client, Guid.Parse(RegulatoryProfileId), SecondClientProfileName, true, clientProfileId);

            //Get all client profile settings for this regulation
            var getClientProfileSettingsRequest = await client.GetAsync($"/api/client-profile-settings");
            var clientProfileSettings = (await getClientProfileSettingsRequest.Content.ReadAsStringAsync())
                .DeserializeJson<GetAllClientProfileSettingsResponse>().ClientProfileSettings;

            //Check if the result contains settings with copied values from the templates
            var containsSettingsCreatedFromProfileTemplate = clientProfileSettings.Any(s =>
                s.IsAvailable && s.AssetTypeId == assetId && s.ClientProfileId == clientProfileId2 &&
                s.Margin == MarginRate);

            var containsSettingsCreatedFromTypeTemplate = clientProfileSettings.Any(s =>
                s.IsAvailable && s.AssetTypeId == assetI2d && s.ClientProfileId == clientProfileId &&
                s.Margin == MarginRate);

            Assert.True(containsSettingsCreatedFromProfileTemplate);
            Assert.True(containsSettingsCreatedFromTypeTemplate);
            Assert.Equal(4, clientProfileSettings.Count);
        }
    }
}