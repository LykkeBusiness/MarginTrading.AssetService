using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.SqlRepositories;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Lykke.Snow.Common.Correlation;

using MarginTrading.AssetService.Tests.Common;

using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public class CurrenciesServiceTests
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;
        private readonly CorrelationContextAccessor _correlationContextAccessor;

        public CurrenciesServiceTests()
        {
            var dbOptions = DatabaseHelper.CreateNewContextOptions<AssetDbContext>(databaseName: "nova");
            _contextFactory = new MsSqlContextFactory<AssetDbContext>(options => new AssetDbContext(options), dbOptions);
            _auditService = new Mock<IAuditService>().Object;
            _cqrsMessageSender = new Mock<ICqrsMessageSender>().Object;
            _convertService = new Mock<IConvertService>().Object;
            _correlationContextAccessor = new Mock<CorrelationContextAccessor>().Object;
        }

        private static CurrencyEntity CreateCurrency(string currencyId)
        {
            var interestRateMdsCode = $"{currencyId}_InterestRatesMdsCode";
            return new CurrencyEntity { Id = currencyId, InterestRateMdsCode = interestRateMdsCode };
        }

        [Fact]
        public async Task Cannot_delete_currency_with_product_referencing_it()
        {
            // Arrange
            const string currencyId = "testCurrencyId";
            const string productId = "testProductId";

            var currency = CreateCurrency(currencyId);
            var product = new ProductEntity
            {
                ProductId = productId, 
                TradingCurrencyId = currencyId, 
                AssetTypeId = "testAssetTypeId",
                CategoryId = "testCategoryId", 
                ForceId = "testForceId",
                IsinLong = "testIsinLong",
                IsinShort = "testIsinShort",
                MarketId = "testMarketId",
                Name = "testName",
                PublicationRic = "testPublicationRic",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TickFormulaId = "testTickFormulaId",
                UnderlyingMdsCode = "testUnderlyingMdsCode"
            };

            await using (var db = _contextFactory.CreateDataContext())
            {
                await db.Currencies.AddAsync(currency);
                await db.Products.AddAsync(product);
                await db.SaveChangesAsync();
            }

            var repository = new CurrenciesRepository(_contextFactory);
            var service = new CurrenciesService(repository, _auditService, _cqrsMessageSender, _convertService, _correlationContextAccessor);

            // Act
            const string userName = "admin";
            var result = await service.DeleteAsync(currencyId, userName);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(Core.Domain.CurrenciesErrorCodes.CannotDeleteCurrencyWithAttachedProducts, result.Error);

            await using (var db = _contextFactory.CreateDataContext())
            {
                Assert.True(await db.Currencies.AnyAsync(x => x.Id == currencyId));
                Assert.True(await db.Products.AnyAsync(x => x.ProductId == productId));
            }
        }

        [Fact]
        public async Task Can_delete_currency_without_product_referencing_it()
        {
            // Arrange
            const string currencyId = "testCurrencyId";
            var currency = CreateCurrency(currencyId);

            await using (var db = _contextFactory.CreateDataContext())
            {
                await db.Currencies.AddAsync(currency);
                await db.SaveChangesAsync();
            }

            var repository = new CurrenciesRepository(_contextFactory);
            var service = new CurrenciesService(repository, _auditService, _cqrsMessageSender, _convertService, _correlationContextAccessor);

            // Act
            const string userName = "admin";
            var result = await service.DeleteAsync(currencyId, userName);

            // Assert
            Assert.True(result.IsSuccess);

            await using (var db = _contextFactory.CreateDataContext())
            {
                Assert.True(await db.Currencies.AllAsync(x => x.Id != currencyId));
            }
        }
    }
}