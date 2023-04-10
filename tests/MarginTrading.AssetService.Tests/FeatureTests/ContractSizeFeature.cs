using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Common;

using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;

using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using MarginTrading.AssetService.Tests.Common;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace MarginTrading.AssetService.Tests.FeatureTests
{
    public class ContractSizeFeature
    {
        private readonly Mock<IUnderlyingsCache> _underlyingsCacheMock = new Mock<IUnderlyingsCache>();
        private readonly Mock<IAssetTypesRepository> _assetTypesRepositoryMock = new Mock<IAssetTypesRepository>();
        private readonly Mock<ILegacyAssetsCache> _legacyAssetsCacheMock = new Mock<ILegacyAssetsCache>();

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(10000)]
        public async Task PostProduct_ThenGetIt_ContractSizeIsSet(int contractSize)
        {
            // mock mds code and asset type validation
            _underlyingsCacheMock.Setup(x => x.GetByMdsCode(It.IsAny<string>()))
                .Returns(new UnderlyingsCacheModel { MdsCode = "mds-code", TradingCurrency = "EUR" });

            _assetTypesRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb(services =>
            {
                services.AddSingleton(_underlyingsCacheMock.Object);
                services.AddSingleton(_assetTypesRepositoryMock.Object);
                services.AddSingleton(_legacyAssetsCacheMock.Object);

                // mock broker settings
                var brokerSettingsApiMock = new Mock<IBrokerSettingsApi>();
                brokerSettingsApiMock
                    .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(new GetBrokerSettingsByIdResponse
                    {
                        ErrorCode = BrokerSettingsErrorCodesContract.None,
                        BrokerSettings = new BrokerSettingsContract { SettlementCurrency = "EUR" }
                    });
                services.AddSingleton(brokerSettingsApiMock.Object);
            });

            // create fake records to be able to create product
            await TestRecordsCreator.CreateCurrencyAsync(client, "EUR");
            await TestRecordsCreator.CreateMarketSettings(client, "market");
            await TestRecordsCreator.CreateTickFormula(client, "tick_formula");
            // create product itself
            await TestRecordsCreator.CreateProductAsync(client, 
                "contract_size_feature_test_product", 
                "stocks",
                contractSize);

            var responseMessage = await client.GetAsync("/api/assetPairs");
            responseMessage.EnsureSuccessStatusCode();
            var assetPair = (await responseMessage.Content.ReadAsStringAsync())
                .DeserializeJson<List<AssetPairContract>>()
                .SingleOrDefault();

            Assert.NotNull(assetPair);
            Assert.Equal(contractSize, assetPair.ContractSize);
        }
        
        [Fact]
        public async Task PostCurrency_ThenGetIt_With_AssetPairsEndpoint_ContractSizeIsSetToDefault()
        {
            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb(services =>
            {
                services.AddSingleton(_underlyingsCacheMock.Object);
                services.AddSingleton(_assetTypesRepositoryMock.Object);
                services.AddSingleton(_legacyAssetsCacheMock.Object);
                
                // mock broker settings
                var brokerSettingsApiMock = new Mock<IBrokerSettingsApi>();
                brokerSettingsApiMock
                    .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(new GetBrokerSettingsByIdResponse
                    {
                        ErrorCode = BrokerSettingsErrorCodesContract.None,
                        BrokerSettings = new BrokerSettingsContract { SettlementCurrency = "EUR" }
                    });
                services.AddSingleton(brokerSettingsApiMock.Object);
            });

            await TestRecordsCreator.CreateCurrencyAsync(client, "my_currency");

            var responseMessage = await client.GetAsync("/api/assetPairs");
            responseMessage.EnsureSuccessStatusCode();
            var currency = (await responseMessage.Content.ReadAsStringAsync())
                .DeserializeJson<List<AssetPairContract>>()
                .SingleOrDefault();

            Assert.NotNull(currency);
            Assert.Equal((int)Currency.DefaultContractSize, currency.ContractSize);
        }
    }
}