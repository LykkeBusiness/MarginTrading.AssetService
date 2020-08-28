using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;
        private readonly IConvertService _convertService;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public ProductsRepository(MsSqlContextFactory<AssetDbContext> contextFactory, IConvertService convertService)
        {
            _contextFactory = contextFactory;
            _convertService = convertService;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product)
        {
            var entity = _convertService.Convert<Product, ProductEntity>(product);

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
            var entity = _convertService.Convert<Product, ProductEntity>(product);

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

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new ProductEntity() {ProductId = productId};

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

                return new Result<Product, ProductsErrorCodes>(_convertService.Convert<ProductEntity, Product>(entity));
            }
        }

        public async Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Products
                    .OrderBy(u => u.Name)
                    .ToListAsync();

                return new Result<List<Product>, ProductsErrorCodes>(
                    _convertService.Convert<List<ProductEntity>, List<Product>>(entities));
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

                return new Result<List<Product>, ProductsErrorCodes>(
                    _convertService.Convert<List<ProductEntity>, List<Product>>(entities));
            }
        }
    }
}