using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Lykke.Common.MsSql;
using Lykke.Snow.Common;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(MsSqlContextFactory<AssetDbContext> contextFactory, ILogger<ProductsRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product)
        {
            var entity = ToEntity(product);

            await using var context = _contextFactory.CreateDataContext();
            await context.Products.AddAsync(entity);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateException e)
            {
                if (e.ValueAlreadyExistsException())
                {
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes.AlreadyExists);
                }

                throw;
            }
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product)
        {
            var entity = ToEntity(product);

            await using var context = _contextFactory.CreateDataContext();
            context.Update(entity);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);
            }
        }

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, byte[] timestamp)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = new ProductEntity { ProductId = productId, Timestamp = timestamp };

            context.Attach(entity);
            context.Products.Remove(entity);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);
            }
        }

        public async Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entity = await context.Products.FindAsync(productId);

            if (entity == null)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

            return new Result<Product, ProductsErrorCodes>(ToModel(entity));
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetByUnderlyingMdsCodeAsync(string mdsCode)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = await context.Products
                .Where(x => x.UnderlyingMdsCode == mdsCode)
                .ToListAsync();

            return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(
            string mdsCodeFilter,
            string[] mdsCodes, string[] productIds,
            bool? isStarted = null, bool? isDiscontinued = null)
        {
            await using var context = _contextFactory.CreateDataContext();
            var query = context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(mdsCodeFilter))
                query = query.Where(x => x.UnderlyingMdsCode.Contains(mdsCodeFilter));

            if (mdsCodes != null && mdsCodes.Any())
                query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

            if (productIds != null && productIds.Any())
                query = query.Where(x => productIds.Contains(x.ProductId));

            if (isStarted.HasValue)
                query = query.Where(x => x.IsStarted == isStarted.Value);
                
            if (isDiscontinued.HasValue)
                query = query.Where(x => x.IsDiscontinued == isDiscontinued.Value);

            var entities = await query
                .OrderBy(u => u.Name)
                .ToListAsync();

            return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
        }

        public async Task<Result<ProductsCounter, ProductsErrorCodes>> GetAllCountAsync(string[] mdsCodes,
            string[] productIds)
        {
            await using var context = _contextFactory.CreateDataContext();
            var query = context.Products.AsNoTracking();

            if (mdsCodes != null && mdsCodes.Any())
                query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

            if (productIds != null && productIds.Any())
                query = query.Where(x => productIds.Contains(x.ProductId));

            var counter = await query.CountAsync();

            return new Result<ProductsCounter, ProductsErrorCodes>(new ProductsCounter(counter, mdsCodes,
                productIds));
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(
            string mdsCodeFilter, string[] mdsCodes,
            string[] productIds, bool? isStarted = null, bool? isDiscontinued = null, int skip = default, int take = 20)
        {

            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            if (take == 0)
            {
                return new Result<List<Product>, ProductsErrorCodes>(new List<Product>());
            }

            await using var context = _contextFactory.CreateDataContext();
            var query = context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(mdsCodeFilter))
                query = query.Where(x => x.UnderlyingMdsCode.Contains(mdsCodeFilter));

            if (mdsCodes != null && mdsCodes.Any())
                query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

            if (productIds != null && productIds.Any())
                query = query.Where(x => productIds.Contains(x.ProductId));

            if (isStarted.HasValue)
                query = query.Where(x => x.IsStarted == isStarted.Value);

            if (isDiscontinued.HasValue)
                query = query.Where(x => x.IsDiscontinued == isDiscontinued.Value);


            var entities = await query
                .OrderBy(u => u.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
        }

        public async Task<Result<Product, ProductsErrorCodes>> ChangeFrozenStatus(string productId, bool isFrozen,
            byte[] valueTimestamp,
            ProductFreezeInfo freezeInfo)
        {
            await using var context = _contextFactory.CreateDataContext();

            var entity = await context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (entity == null)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

            entity.IsFrozen = isFrozen;
            entity.FreezeInfo = JsonConvert.SerializeObject(freezeInfo);

            var saved = false;

            while(!saved)
            {
                try
                {
                    await context.SaveChangesAsync();

                    saved = true;
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    // https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations
                    var entry = ex.Entries.FirstOrDefault();

                    var attemptedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in attemptedValues.Properties)
                    {
                        var attemptedValue = attemptedValues[property];
                        var databaseValue = databaseValues[property];
                        
                        var updatedFields = new HashSet<string>()
                        {
                            nameof(ProductEntity.IsFrozen),
                            nameof(ProductEntity.FreezeInfo)
                        };

                        // Leave updated columns within this method as they are 
                        // while we set the rest of the properties to the database values.
                        if(updatedFields.Contains(property.Name))
                            continue;
                        
                        // Here we update our attempted values with the values 
                        // That's been updated by another thread/process so that 
                        // we can merge the changes without overriding them with stale values.
                        attemptedValues[property] = databaseValue;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
            
            return new Result<Product, ProductsErrorCodes>(ToModel(entity));
        }

        public async Task<Result<ProductsErrorCodes>> UpdateBatchAsync(List<Product> products)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = products.Select(ToEntity);

            context.UpdateRange(entities);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                _logger.LogError(e, "Failed to batch update products");
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);
            }
        }

        public async Task<Result<ProductsErrorCodes>> DeleteBatchAsync(
            Dictionary<string, byte[]> productIdsWithTimestamps)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = productIdsWithTimestamps
                .Select(kvp => new ProductEntity { ProductId = kvp.Key, Timestamp = kvp.Value })
                .ToArray();
            
            context.Products.RemoveRange(entities);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateConcurrencyException e) when (e.IsMissingDataException())
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);
            }
        }

        public async Task<Dictionary<string, string>> GetProductAssetTypeMapAsync(IEnumerable<string> productIds = null)
        {
            await using var context = _contextFactory.CreateDataContext();
            var query = context.Products.AsNoTracking();

            if (productIds != null && productIds.Any())
                query = query.Where(x => productIds.Contains(x.ProductId));

            var result = await query.ToDictionaryAsync(k => k.ProductId, v => v.AssetTypeId);

            return result;
        }

        public async Task<IReadOnlyList<Product>> GetByProductsIdsAsync(IEnumerable<string> productIds = null, bool startedOnly = true)
        {
            await using var context = _contextFactory.CreateDataContext();
            var query = context
                .Products
                .AsNoTracking()
                .Where(x => !startedOnly || x.IsStarted);

            if (productIds != null && productIds.Any())
                query = query.Where(x => productIds.Contains(x.ProductId));

            var result = await query.ToListAsync();

            return result.Select(ToModel).ToList();
        }

        public async Task<PaginatedResponse<Product>> GetPagedByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds,
            int skip = default, int take = 20)
        {

            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            await using var context = _contextFactory.CreateDataContext();
            var query = context.Products.Where(p => assetTypeIds.Contains(p.AssetTypeId) && p.IsStarted);

            var total = await query.CountAsync();
            var products = await query
                .OrderBy(u => u.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PaginatedResponse<Product>(products.Select(ToModel).ToList(), skip, products.Count, total);
        }

        public async Task<IReadOnlyList<Product>> GetByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds)
        {
            await using var context = _contextFactory.CreateDataContext();
            var products = await context.Products
                .Where(p => assetTypeIds.Contains(p.AssetTypeId) && p.IsStarted)
                .ToListAsync();

            return products.Select(ToModel).ToList();
        }

        public async Task MarkAsDiscontinuedAsync(IEnumerable<string> productIds)
        {
            await using var context = _contextFactory.CreateDataContext();
            var items = productIds
                .Select((x, i) => new SqlParameter($"@p{i}", x));
            var sql =
                $"UPDATE [dbo].[Products] SET IsDiscontinued = 1 WHERE ProductId IN ({string.Join(", ", items.Select(x => x.ParameterName))})"; 
            await context.Database.ExecuteSqlRawAsync(sql, items);
        }

        public async Task<Result<Product, ProductsErrorCodes>> ChangeSuspendFlagAsync(string id, bool value)
        {
            await using var context = _contextFactory.CreateDataContext();

            var product = await context.Products.FindAsync(id);

            if (product == null)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

            _logger.LogInformation(@$"🚩 Product {id} is read from the database. 
                IsSuspended: {product.IsSuspended} - IsFrozen: {product.IsFrozen}
                Now stopping the thread for 15 seconds to simulate concurrency....");
                
            Thread.Sleep(TimeSpan.FromSeconds(15));

            product.IsSuspended = value;
            
            var saved = false;
            while (!saved)
            {
                try
                {
                    await context.SaveChangesAsync();

                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations
                    var entry = ex.Entries.FirstOrDefault();

                    var attemptedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in attemptedValues.Properties)
                    {
                        var proposedValue = attemptedValues[property];
                        var databaseValue = databaseValues[property];

                        // Leave updated columns within this method as they are 
                        // while we set the rest of the properties to the database values.
                        if(property.Name == nameof(ProductEntity.IsSuspended))
                            continue;

                        // Here we update our attempted values with the values 
                        // That's been updated by another thread/process so that 
                        // we can merge the changes without overriding them with stale values.
                        attemptedValues[property] = databaseValue;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
            
            return new Result<Product, ProductsErrorCodes>(ToModel(product));
        }

        private static ProductEntity ToEntity(Product product)
        {
            var result = new ProductEntity
            {
                ProductId = product.ProductId,
                AssetTypeId = product.AssetType,
                CategoryId = product.Category,
                Comments = product.Comments,
                ContractSize = product.ContractSize,
                IsinLong = product.IsinLong,
                IsinShort = product.IsinShort,
                Issuer = product.Issuer,
                MarketId = product.Market,
                MarketMakerAssetAccountId = product.MarketMakerAssetAccountId,
                MaxOrderSize = product.MaxOrderSize,
                MinOrderSize = product.MinOrderSize,
                MaxPositionSize = product.MaxPositionSize,
                MinOrderDistancePercent = product.MinOrderDistancePercent,
                Name = product.Name,
                NewsId = product.NewsId,
                Keywords = product.Keywords,
                PublicationRic = product.PublicationRic,
                SettlementCurrency = product.SettlementCurrency,
                ShortPosition = product.ShortPosition,
                TickFormulaId = product.TickFormula,
                UnderlyingMdsCode = product.UnderlyingMdsCode,
                ForceId = product.ForceId,
                Parity = product.Parity,
                OvernightMarginMultiplier = product.OvernightMarginMultiplier,
                TradingCurrencyId = product.TradingCurrency,
                IsSuspended = product.IsSuspended,
                IsFrozen = product.IsFrozen,
                FreezeInfo = product.FreezeInfo.ToJson(),
                IsDiscontinued = product.IsDiscontinued,
                Timestamp = product.Timestamp,
                StartDate = product.StartDate,
                IsStarted = product.IsStarted,
                Dividends871M = product.Dividends871M,
                DividendsLong = product.DividendsLong,
                DividendsShort = product.DividendsShort,
                HedgeCost = product.HedgeCost,
                EnforceMargin = product.EnforceMargin,
                Margin = product.Margin,
                MaxPositionNotional = product.MaxPositionNotional,
                IsTradingDisabled = product.IsTradingDisabled
            };

            return result;
        }

        private static Product ToModel(ProductEntity product)
        {
            var result = new Product
            {
                ProductId = product.ProductId,
                AssetType = product.AssetTypeId,
                Category = product.CategoryId,
                Comments = product.Comments,
                ContractSize = product.ContractSize,
                IsinLong = product.IsinLong,
                IsinShort = product.IsinShort,
                Issuer = product.Issuer,
                Market = product.MarketId,
                MarketMakerAssetAccountId = product.MarketMakerAssetAccountId,
                MaxOrderSize = product.MaxOrderSize,
                MinOrderSize = product.MinOrderSize,
                MaxPositionSize = product.MaxPositionSize,
                MinOrderDistancePercent = product.MinOrderDistancePercent,
                Name = product.Name,
                NewsId = product.NewsId,
                Keywords = product.Keywords,
                PublicationRic = product.PublicationRic,
                SettlementCurrency = product.SettlementCurrency,
                ShortPosition = product.ShortPosition,
                TickFormula = product.TickFormulaId,
                UnderlyingMdsCode = product.UnderlyingMdsCode,
                ForceId = product.ForceId,
                Parity = product.Parity,
                OvernightMarginMultiplier = product.OvernightMarginMultiplier,
                TradingCurrency = product.TradingCurrencyId,
                IsSuspended = product.IsSuspended,
                IsFrozen = product.IsFrozen,
                FreezeInfo = product.FreezeInfo?.DeserializeJson<ProductFreezeInfo>(),
                IsDiscontinued = product.IsDiscontinued,
                Timestamp = product.Timestamp,
                StartDate = product.StartDate,
                IsStarted = product.IsStarted,
                Dividends871M = product.Dividends871M,
                DividendsLong = product.DividendsLong,
                DividendsShort = product.DividendsShort,
                HedgeCost = product.HedgeCost,
                EnforceMargin = product.EnforceMargin,
                Margin = product.Margin,
                MaxPositionNotional = product.MaxPositionNotional,
                IsTradingDisabled = product.IsTradingDisabled
            };

            return result;
        }

        public async Task<(bool result, string id)> IsinExists(string isin, bool? isDiscontinued = null)
        {
            await using var context = _contextFactory.CreateDataContext();
            var filteredQuery = context.Products
                .Where(p => p.IsinLong == isin || p.IsinShort == isin);

            if (isDiscontinued.HasValue)
            {
                filteredQuery = filteredQuery
                    .Where(x => x.IsDiscontinued == isDiscontinued.Value);
            }

            var id = await filteredQuery
                .Select(p => p.ProductId)
                .FirstOrDefaultAsync();

            return (!string.IsNullOrWhiteSpace(id), id);
        }
    }
}