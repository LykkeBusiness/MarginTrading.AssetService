using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common;

using Lykke.Contracts.Responses;

using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Tests.Extensions;
using Newtonsoft.Json;

using Xunit;

namespace MarginTrading.AssetService.Tests.Common
{
    public class TestRecordsCreator
    {
        public static async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> CreateAssetTypeAsync(
            HttpClient client,
            string regulatoryTypeId,
            string id,
            string underlyingCategoryId,
            string assetTypeTemplateId = null)
        {
            var request = new AddAssetTypeRequest
            {
                RegulatoryTypeId = regulatoryTypeId,
                Username = "asdasd",
                AssetTypeTemplateId = assetTypeTemplateId,
                Id = id,
                UnderlyingCategoryId = underlyingCategoryId
            };

            var response = await client.PostAsync($"/api/asset-types", request.ToJsonStringContent());
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ErrorCodeResponse<ClientProfilesErrorCodesContract>>(content);
        }

        public static async Task<ErrorCodeResponse<ClientProfilesErrorCodesContract>> CreateClientProfileAsync(HttpClient client, string regulatoryProfileId, string id, bool isDefault, string clientProfileTemplateId = null)
        {
            var request = new AddClientProfileRequest
            {
                RegulatoryProfileId = regulatoryProfileId,
                Username = "asdasd",
                IsDefault = isDefault,
                ClientProfileTemplateId = clientProfileTemplateId,
                Id = id
            };

            var response = await client.PostAsync($"/api/client-profiles", request.ToJsonStringContent());
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ErrorCodeResponse<ClientProfilesErrorCodesContract>>(content);
        }

        public static async Task<ProductCategoriesErrorCodesContract> CreateCategoryAsync(HttpClient client, string category)
        {
            var request = new AddProductCategoryRequest
            {
                Category = category,
                UserName = "user"
            };

            var response = await client.PostAsync("/api/product-categories", request.ToJsonStringContent());
            var errorCode = (await response.Content.ReadAsStringAsync())
                .DeserializeJson<ErrorCodeResponse<ProductCategoriesErrorCodesContract>>().ErrorCode;

            return errorCode;
        }

        public static async Task CreateCurrencyAsync(HttpClient client, string id)
        {
            var request = new AddCurrencyRequest
            {
                Id = id,
                InterestRateMdsCode = id,
                UserName = "username"
            };

            var response = await client.PostAsync("/api/currencies", request.ToJsonStringContent());
            response.EnsureSuccessStatusCode();
            
            var result = (await response.Content.ReadAsStringAsync())
                .DeserializeJson<ErrorCodeResponse<CurrenciesErrorCodesContract>>();
            Assert.Equal(CurrenciesErrorCodesContract.None, result.ErrorCode);
        }

        public static async Task CreateMarketSettings(HttpClient client, string id)
        {
            var request = new AddMarketSettingsRequest
            {
                Id = id,
                Name = id,
                Username = "username",
                Timezone = "UTC",
                Holidays = new List<DateTime>(),
                Open = new[] {new TimeSpan(8, 0, 0)},
                Close = new[] {new TimeSpan(20, 0, 0)},
                HalfWorkingDays = new List<string>()
            };

            await client.PostAsync("/api/market-settings", request.ToJsonStringContent());
        }

        public static async Task CreateTickFormula(HttpClient client, string id)
        {
            var request = new AddTickFormulaRequest
            {
                Id = id,
                Username = "username",
                PdlLadders = new List<decimal>
                {
                    {new decimal(0)}, {1}
                },
                PdlTicks = new List<decimal>
                {
                    {new decimal(0.1)}, {1}
                }
            };

            await client.PostAsync("/api/tick-formulas", request.ToJsonStringContent());
        }

        public static async Task<ErrorCodeResponse<ProductsErrorCodesContract>> CreateProductAsync(HttpClient client,
            string productId, string category, int contractSize = 1, bool started = true)
        {
            var request = new AddProductRequest
            {
                ProductId = productId,
                Category = category,
                AssetType = "asset-type",
                ContractSize = contractSize,
                Comments = "comments",
                Issuer = "issuer",
                Keywords = "keywords",
                Market = "market",
                Name = productId,
                Parity = 10,
                ForceId = "forceId",
                IsinLong = "IsinLong1234",
                IsinShort = "IsinShort123",
                NewsId = nameof(AddProductRequest.NewsId),
                PublicationRic = nameof(AddProductRequest.PublicationRic),
                ShortPosition = true,
                SettlementCurrency = nameof(AddProductRequest.SettlementCurrency),
                TickFormula = "tick_formula",
                UserName = "username",
                MaxOrderSize = 1,
                MaxPositionSize = 1,
                MinOrderSize = 1,
                OvernightMarginMultiplier = new decimal(1.0),
                UnderlyingMdsCode = "mds-code",
                MinOrderDistancePercent = new decimal(1.0),
                MarketMakerAssetAccountId = nameof(AddProductRequest.MarketMakerAssetAccountId),
                StartDate = started ? DateTime.UtcNow.AddDays(-1) : DateTime.UtcNow.AddDays(1)
            };

            var response = await client.PostAsync("/api/products", request.ToJsonStringContent());
            response.EnsureSuccessStatusCode();

            var result = (await response.Content.ReadAsStringAsync())
                .DeserializeJson<ErrorCodeResponse<ProductsErrorCodesContract>>();
            Assert.Equal(ProductsErrorCodesContract.None, result.ErrorCode);

            return result;
        }
    }
}