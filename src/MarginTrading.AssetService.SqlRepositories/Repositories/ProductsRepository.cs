using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public ProductsRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product)
        {
            var entity = ToEntity(product);

            using (var context = _contextFactory.CreateDataContext())
            {
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
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product)
        {
            var entity = ToEntity(product);

            using (var context = _contextFactory.CreateDataContext())
            {
                context.Update(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<ProductsErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.Message.Contains(DoesNotExistException))
                        return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                    throw;
                }
            }
        }

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, byte[] timestamp)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new ProductEntity() {ProductId = productId, Timestamp = timestamp};

                context.Attach(entity);
                context.Products.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<ProductsErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.Message.Contains(DoesNotExistException))
                        return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                    throw;
                }
            }
        }

        public async Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.Products.FindAsync(productId);

                if (entity == null)
                    return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                return new Result<Product, ProductsErrorCodes>(ToModel(entity));
            }
        }

        public async Task<Result<Product, ProductsErrorCodes>> GetByUnderlyingMdsCodeAsync(string mdsCode)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.Products.FirstOrDefaultAsync(x => x.UnderlyingMdsCode == mdsCode);

                if (entity == null)
                    return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                return new Result<Product, ProductsErrorCodes>(ToModel(entity));
            }
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted = null)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.AsNoTracking();

                if (mdsCodes != null && mdsCodes.Any())
                    query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

                if (productIds != null && productIds.Any())
                    query = query.Where(x => productIds.Contains(x.ProductId));

                if (isStarted.HasValue)
                    query = query.Where(x => x.IsStarted == isStarted.Value);

                var entities = await query
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
            }
        }

        public async Task<Result<ProductsCounter, ProductsErrorCodes>> GetAllCountAsync(string[] mdsCodes,
            string[] productIds)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.AsNoTracking();

                if (mdsCodes != null && mdsCodes.Any())
                    query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

                if (productIds != null && productIds.Any())
                    query = query.Where(x => productIds.Contains(x.ProductId));

                var counter = await query.CountAsync();

                return new Result<ProductsCounter, ProductsErrorCodes>(new ProductsCounter(counter, mdsCodes,
                    productIds));
            }
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(string[] mdsCodes,
            string[] productIds, bool? isStarted = null, int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            if (take == 0)
            {
                return new Result<List<Product>, ProductsErrorCodes>(new List<Product>());
            }

            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.AsNoTracking();

                if (mdsCodes != null && mdsCodes.Any())
                    query = query.Where(x => mdsCodes.Contains(x.UnderlyingMdsCode));

                if (productIds != null && productIds.Any())
                    query = query.Where(x => productIds.Contains(x.ProductId));

                if (isStarted.HasValue)
                    query = query.Where(x => x.IsStarted == isStarted.Value);


                var entities = await query
                    .OrderBy(u => u.Name)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
            }
        }

        public async Task<bool> UnderlyingHasOnlyOneProduct(string mdsCode, string productId)
        {
            await using var context = _contextFactory.CreateDataContext();

            var result = await context.Products
                .AnyAsync(p => p.UnderlyingMdsCode == mdsCode
                               && p.ProductId != productId);

            return !result;
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

            await context.SaveChangesAsync();

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
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains(DoesNotExistException))
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                throw;
            }
        }

        public async Task<Result<ProductsErrorCodes>> DeleteBatchAsync(
            Dictionary<string, byte[]> productIdsWithTimestamps)
        {
            await using var context = _contextFactory.CreateDataContext();
            var entities = productIdsWithTimestamps.Select(kvp =>
                    new ProductEntity() {ProductId = kvp.Key, Timestamp = kvp.Value})
                .ToArray();

            context.AttachRange(entities);
            context.Products.RemoveRange(entities);

            try
            {
                await context.SaveChangesAsync();
                return new Result<ProductsErrorCodes>();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains(DoesNotExistException))
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                throw;
            }
        }

        public async Task<Dictionary<string, string>> GetProductAssetTypeMapAsync(IEnumerable<string> productIds = null)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.AsNoTracking();

                if (productIds != null && productIds.Any())
                    query = query.Where(x => productIds.Contains(x.ProductId));

                var result = await query.ToDictionaryAsync(k => k.ProductId, v => v.AssetTypeId);

                return result;
            }
        }

        public async Task<IReadOnlyList<Product>> GetByProductsIdsAsync(IEnumerable<string> productIds = null)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.AsNoTracking();

                if (productIds != null && productIds.Any())
                    query = query.Where(x => productIds.Contains(x.ProductId) && x.IsStarted);

                var result = await query.ToListAsync();

                return result.Select(ToModel).ToList();
            }
        }

        public async Task<PaginatedResponse<Product>> GetPagedByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds,
            int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.Where(p => assetTypeIds.Contains(p.AssetTypeId) && p.IsStarted);

                var total = await query.CountAsync();
                var products = await query
                    .OrderBy(u => u.Name)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return new PaginatedResponse<Product>(products.Select(ToModel).ToList(), skip, products.Count, total);
            }
        }

        public async Task<IReadOnlyList<Product>> GetByAssetTypeIdsAsync(IEnumerable<string> assetTypeIds)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var products = await context.Products
                    .Where(p => assetTypeIds.Contains(p.AssetTypeId) && p.IsStarted)
                    .ToListAsync();

                return products.Select(ToModel).ToList();
            }
        }

        public async Task<Result<Product, ProductsErrorCodes>> ChangeSuspendFlagAsync(string id, bool value)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var product = await context.Products.FindAsync(id);

                if (product == null)
                    return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);

                product.IsSuspended = value;
                context.Products.Update(product);

                await context.SaveChangesAsync();
                return new Result<Product, ProductsErrorCodes>(ToModel(product));
            }
        }

        private static ProductEntity ToEntity(Product product)
        {
            var result = new ProductEntity()
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
                MinOrderEntryInterval = product.MinOrderEntryInterval,
                Name = product.Name,
                NewsId = product.NewsId,
                Keywords = product.Keywords,
                PublicationRic = product.PublicationRic,
                SettlementCurrency = product.SettlementCurrency,
                ShortPosition = product.ShortPosition,
                Tags = product.Tags,
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
                StartDate = product.StartDate.Value,
                IsStarted = product.IsStarted,
            };

            return result;
        }

        private static Product ToModel(ProductEntity product)
        {
            var result = new Product()
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
                MinOrderEntryInterval = product.MinOrderEntryInterval,
                Name = product.Name,
                NewsId = product.NewsId,
                Keywords = product.Keywords,
                PublicationRic = product.PublicationRic,
                SettlementCurrency = product.SettlementCurrency,
                ShortPosition = product.ShortPosition,
                Tags = product.Tags,
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
            };

            return result;
        }
    }
}