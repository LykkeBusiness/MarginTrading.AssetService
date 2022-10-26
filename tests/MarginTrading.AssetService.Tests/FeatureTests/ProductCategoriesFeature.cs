using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using MarginTrading.AssetService.Tests.Common;
using MarginTrading.AssetService.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests.FeatureTests
{
    public class ProductCategoriesFeature
    {
        private readonly Mock<IUnderlyingsCache> _underlyingsCacheMock = new Mock<IUnderlyingsCache>();
        private readonly Mock<IAssetTypesRepository> _assetTypesRepositoryMock = new Mock<IAssetTypesRepository>();

        [Fact]
        public async Task ProductCategoriesWorkflow()
        {
            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb();

            await TestRecordsCreator.CreateCategoryAsync(client, "stocks/Germany/Dax 30");

            var getCategoriesResponse = await client.GetAsync("/api/product-categories");
            var categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Equal(3, categories.Count);
            Assert.True(categories.First(c => c.Id == "stocks.germany.dax_30").IsLeaf);
            Assert.False(categories.First(c => c.Id == "stocks.germany").IsLeaf);
            Assert.False(categories.First(c => c.Id == "stocks").IsLeaf);

            var deleteRequest = new DeleteProductCategoryRequest
            {
                UserName = "user"
            };

            var leafCategoryId = "stocks.germany.dax_30";

            await client.DeleteAsJsonAsync($"/api/product-categories/{leafCategoryId}", deleteRequest);

            getCategoriesResponse = await client.GetAsync("/api/product-categories");
            categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Equal(2, categories.Count);
            Assert.True(categories.First(c => c.Id == "stocks.germany").IsLeaf);
            Assert.False(categories.First(c => c.Id == "stocks").IsLeaf);
        }

        [Fact]
        public async Task ProductCategories_CannotDeleteCategoryWithAttachedProducts_Workflow()
        {
            _underlyingsCacheMock.Setup(x => x.GetByMdsCode(It.IsAny<string>()))
                .Returns(new UnderlyingsCacheModel
                {
                    MdsCode = "mds-code",
                    TradingCurrency = "EUR"
                });

            _assetTypesRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb(services =>
            {
                services.AddSingleton(_underlyingsCacheMock.Object);
                services.AddSingleton(_assetTypesRepositoryMock.Object);
            });

            var category = "stocks";

            await TestRecordsCreator.CreateCurrencyAsync(client, "EUR");
            await TestRecordsCreator.CreateMarketSettings(client, "market");
            await TestRecordsCreator.CreateTickFormula(client, "tick_formula");
            // asset type is mocked
            await TestRecordsCreator.CreateProductAsync(client, "product1", category);

            var getCategoriesResponse = await client.GetAsync("/api/product-categories");
            var categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Single(categories);

            var deleteRequest = new DeleteProductCategoryRequest
            {
                UserName = "username"
            };

            var deleteCategoryResponse =
                (await client.DeleteAsJsonAsync($"/api/product-categories/{category}", deleteRequest));
            var errorCode = (await deleteCategoryResponse.Content.ReadAsStringAsync())
                .DeserializeJson<ErrorCodeResponse<ProductCategoriesErrorCodesContract>>().ErrorCode;

            Assert.Equal(ProductCategoriesErrorCodesContract.CannotDeleteCategoryWithAttachedProducts, errorCode);

            getCategoriesResponse = await client.GetAsync("/api/product-categories");
            categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Single(categories);
        }

        [Fact]
        public async Task ProductCategories_CannotDeleteNonLeafCategory_Workflow()
        {
            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb();

            var notLeafCategory = "stocks";
            var category = "stocks/germany";

            await TestRecordsCreator.CreateCategoryAsync(client, category);

            var getCategoriesResponse = await client.GetAsync("/api/product-categories");
            var categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Collection(categories, 
                x => { Assert.Equal("stocks", x.Id); },
                x1 => { Assert.Equal("stocks.germany", x1.Id); });

            var deleteRequest = new DeleteProductCategoryRequest
            {
                UserName = "username"
            };

            var deleteCategoryResponse =
                (await client.DeleteAsJsonAsync($"/api/product-categories/{notLeafCategory}", deleteRequest));
            var errorCode = (await deleteCategoryResponse.Content.ReadAsStringAsync())
                .DeserializeJson<ErrorCodeResponse<ProductCategoriesErrorCodesContract>>().ErrorCode;

            Assert.Equal(ProductCategoriesErrorCodesContract.CannotDeleteNonLeafCategory, errorCode);

            getCategoriesResponse = await client.GetAsync("/api/product-categories");
            categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Collection(categories,
                x => { Assert.Equal("stocks", x.Id); },
                x1 => { Assert.Equal("stocks.germany", x1.Id); });
        }

        [Fact]
        public async Task ProductCategories_ParentHasAttachedProducts_Workflow()
        {
            _underlyingsCacheMock.Setup(x => x.GetByMdsCode(It.IsAny<string>()))
                .Returns(new UnderlyingsCacheModel
                {
                    MdsCode = "mds-code",
                    TradingCurrency = "EUR"
                });

            _assetTypesRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            using var client = TestBootstrapper.CreateTestClientWithInMemoryDb(services =>
            {
                services.AddSingleton(_underlyingsCacheMock.Object);
                services.AddSingleton(_assetTypesRepositoryMock.Object);
            });

            var categoryWithProduct = "stocks/germany";
            var category = "stocks/germany/dax 30";

            await TestRecordsCreator.CreateCurrencyAsync(client, "EUR");
            await TestRecordsCreator.CreateMarketSettings(client, "market");
            await TestRecordsCreator.CreateTickFormula(client, "tick_formula");
            // asset type is mocked
            await TestRecordsCreator.CreateProductAsync(client, "product1", categoryWithProduct);

            var errorCode = await TestRecordsCreator.CreateCategoryAsync(client, category);

            Assert.Equal(ProductCategoriesErrorCodesContract.ParentHasAttachedProducts, errorCode);

            var getCategoriesResponse = await client.GetAsync("/api/product-categories");
            var categories = (await getCategoriesResponse.Content.ReadAsStringAsync())
                .DeserializeJson<GetProductCategoriesResponse>().ProductCategories;

            Assert.Collection(categories,
                x => { Assert.Equal("stocks", x.Id); },
                x1 => { Assert.Equal("stocks.germany", x1.Id); });
        }
    }
}