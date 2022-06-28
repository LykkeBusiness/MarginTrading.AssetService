using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.SqlRepositories;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using Lykke.Snow.Common.Correlation;
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
        private readonly IIdentityGenerator _identityGenerator;

        public CurrenciesServiceTests()
        {
            var dbOptions = CreateNewContextOptions<AssetDbContext>(databaseName: "nova");
            _contextFactory = new MsSqlContextFactory<AssetDbContext>((options) => new AssetDbContext(options), dbOptions);
            _auditService = new Mock<IAuditService>().Object;
            _cqrsMessageSender = new Mock<ICqrsMessageSender>().Object;
            _convertService = new Mock<IConvertService>().Object;
            _correlationContextAccessor = new Mock<CorrelationContextAccessor>().Object;
            _identityGenerator = new Mock<IIdentityGenerator>().Object;
        }

        private static DbContextOptions<T> CreateNewContextOptions<T>(string databaseName) where T : DbContext
        {
            // https://stackoverflow.com/questions/38890269/how-to-isolate-ef-inmemory-database-per-xunit-test
            // Typically, EF creates a single IServiceProvider for all contexts
            // of a given type in an AppDomain - meaning all context instances
            // share the same InMemory database instance. By allowing one to be
            // passed in, you can control the scope of the InMemory database.

            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseInMemoryDatabase(databaseName: databaseName)
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
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
            var product = new ProductEntity { ProductId = productId, TradingCurrencyId = currencyId };

            using (var db = _contextFactory.CreateDataContext())
            {
                await db.Currencies.AddAsync(currency);
                await db.Products.AddAsync(product);
                await db.SaveChangesAsync();
            }

            var repository = new CurrenciesRepository(_contextFactory);
            var service = new CurrenciesService(repository, _auditService, _cqrsMessageSender, _convertService, _correlationContextAccessor);

            // Act
            const string userName = "admin";
            string correlationId = Guid.NewGuid().ToString("N");
            var result = await service.DeleteAsync(currencyId, userName);

            // Assert
            Assert.True(result.IsFailed);
            Assert.Equal(Core.Domain.CurrenciesErrorCodes.CannotDeleteCurrencyWithAttachedProducts, result.Error);

            using (var db = _contextFactory.CreateDataContext())
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

            using (var db = _contextFactory.CreateDataContext())
            {
                await db.Currencies.AddAsync(currency);
                await db.SaveChangesAsync();
            }

            var repository = new CurrenciesRepository(_contextFactory);
            var service = new CurrenciesService(repository, _auditService, _cqrsMessageSender, _convertService, _correlationContextAccessor);

            // Act
            const string userName = "admin";
            string correlationId = Guid.NewGuid().ToString("N");
            var result = await service.DeleteAsync(currencyId, userName);

            // Assert
            Assert.True(result.IsSuccess);

            using (var db = _contextFactory.CreateDataContext())
            {
                Assert.True(await db.Currencies.AllAsync(x => x.Id != currencyId));
            }
        }
    }
}