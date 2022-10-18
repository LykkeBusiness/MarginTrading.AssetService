using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lykke.Common.MsSql;

using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories;
using MarginTrading.AssetService.SqlRepositories.Repositories;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using MarginTrading.AssetService.Tests.Common;

using Microsoft.Extensions.Logging.Abstractions;

using Xunit;

using DateOnly = Lykke.Snow.Common.Types.DateOnly;

namespace MarginTrading.AssetService.Tests
{
    /// <summary>
    /// Multiple repositories are using <see cref="Lykke.Common.MsSql.ExceptionExtensions"/> to detect missing data
    /// exception. This test ensures that when data is missed we are not getting an exception but a result object with
    /// corresponding error code.
    /// Since the approach is the same across all repositories we are testing only one of them. 
    /// </summary>
    public class RepositoryDataMissingTests
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public RepositoryDataMissingTests()
        {
            var dbOptions = DatabaseHelper.CreateNewContextOptions<AssetDbContext>(databaseName: "nova");
            _contextFactory =
                new MsSqlContextFactory<AssetDbContext>(options => new AssetDbContext(options), dbOptions);
        }
        
        [Fact]
        public async Task Delete_DataMissing_Returns_Corresponding_Error_Code()
        {
            var repo = CreateSut();

            var result = await repo.DeleteAsync("id-does-not-exist", BitConverter.GetBytes(DateTime.UtcNow.Ticks));

            Assert.Equal(ProductsErrorCodes.DoesNotExist, result.Error);
        }
        
        [Fact]
        public async Task DeleteBatch_DataMissing_Returns_Corresponding_Error_Code()
        {
            var repo = CreateSut();

            var result = await repo.DeleteBatchAsync(new Dictionary<string, byte[]>
            {
                { "id-does-not-exist", BitConverter.GetBytes(DateTime.UtcNow.Ticks) }
            });

            Assert.Equal(ProductsErrorCodes.DoesNotExist, result.Error);
        }
        
        [Fact]
        public async Task UpdateBatch_DataMissing_Returns_Corresponding_Error_Code()
        {
            var repo = CreateSut();

            var result =
                await repo.UpdateBatchAsync(new List<Product>
                {
                    new Product
                    {
                        ProductId = "id-does-not-exist", 
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    }
                });

            Assert.Equal(ProductsErrorCodes.DoesNotExist, result.Error);
        }
        
        [Fact]
        public async Task Update_DataMissing_Returns_Corresponding_Error_Code()
        {
            var repo = CreateSut();

            var result = await repo.UpdateAsync(
                new Product { ProductId = "id-does-not-exist", StartDate = DateOnly.FromDateTime(DateTime.UtcNow) });

            Assert.Equal(ProductsErrorCodes.DoesNotExist, result.Error);
        }
        
        

        private IProductsRepository CreateSut()
        {
            return new ProductsRepository(_contextFactory, NullLogger<ProductsRepository>.Instance);
        }
    }
}