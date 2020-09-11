using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Products
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
            }
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(int skip = default, int take = 20)
        {
            skip = Math.Max(0, skip);
            take = take < 0 ? 20 : Math.Min(take, 100);

            if (take == 0)
            {
                return new Result<List<Product>, ProductsErrorCodes>(new List<Product>());
            }

            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Products
                    .OrderBy(u => u.Name)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return new Result<List<Product>, ProductsErrorCodes>(entities.Select(ToModel).ToList());
            }
        }

        private static ProductEntity ToEntity(Product product)
        {
            var result = new ProductEntity()
            {
                ProductId = product.ProductId,
                AssetType = product.AssetType,
                CategoryId = product.Category,
                Comments = product.Comments,
                ContractSize = product.ContractSize,
                IsinLong = product.IsinLong,
                IsinShort = product.IsinShort,
                Issuer = product.Issuer,
                Market = product.Market,
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
                TickFormula = product.TickFormula,
                UnderlyingMdsCode = product.UnderlyingMdsCode,
                ForceId = product.ForceId,
                Parity = product.Parity,
                OvernightMarginMultiplier = product.OvernightMarginMultiplier,
                Timestamp = product.Timestamp,
            };

            return result;
        }

        private static Product ToModel(ProductEntity product)
        {
            var result = new Product()
            {
                ProductId = product.ProductId,
                AssetType = product.AssetType,
                Category = product.CategoryId,
                Comments = product.Comments,
                ContractSize = product.ContractSize,
                IsinLong = product.IsinLong,
                IsinShort = product.IsinShort,
                Issuer = product.Issuer,
                Market = product.Market,
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
                TickFormula = product.TickFormula,
                UnderlyingMdsCode = product.UnderlyingMdsCode,
                ForceId = product.ForceId,
                Parity = product.Parity,
                OvernightMarginMultiplier = product.OvernightMarginMultiplier,
                Timestamp = product.Timestamp,
            };

            return result;
        }
    }
}