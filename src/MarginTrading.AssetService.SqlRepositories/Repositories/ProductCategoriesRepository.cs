using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class ProductCategoriesRepository : IProductCategoriesRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public ProductCategoriesRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Result<ProductCategoriesErrorCodes>> InsertAsync(ProductCategory category)
        {
            var entity = ToEntity(category);

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.ProductCategories.AddAsync(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<ProductCategoriesErrorCodes>();
                }
                catch (DbUpdateException e)
                {
                    if (e.ValueAlreadyExistsException())
                    {
                        return new Result<ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes.AlreadyExists);
                    }

                    throw;
                }
            }
        }

        public async Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, byte[] timestamp)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = new ProductCategoryEntity() {Id = id, Timestamp = timestamp};

                context.Attach(entity);
                context.ProductCategories.Remove(entity);

                try
                {
                    await context.SaveChangesAsync();
                    return new Result<ProductCategoriesErrorCodes>();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (e.Message.Contains(DoesNotExistException))
                        return new Result<ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes.DoesNotExist);

                    throw;
                }
            }
        }

        public async Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.ProductCategories
                    .Include(x => x.Children)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return new Result<ProductCategory, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                        .DoesNotExist);

                return new Result<ProductCategory, ProductCategoriesErrorCodes>(ToModel(entity));
            }
        }

        public async Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.ProductCategories
                    .ToListAsync();

                return new Result<List<ProductCategory>, ProductCategoriesErrorCodes>(entities.Select(ToModel)
                    .ToList());
            }
        }

        public async Task<bool> CategoryHasProducts(string category)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var hasProducts = await context.Products.AnyAsync(x => x.CategoryId == category);
                return hasProducts;
            }
        }

        public async Task<IReadOnlyList<ProductCategory>> GetByIdsAsync(IEnumerable<string> ids)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.ProductCategories.AsNoTracking();

                if (ids != null && ids.Any())
                    query = query.Where(x => ids.Contains(x.Id));

                var result = await query
                    .ToListAsync();

                return result.Select(ToModel).ToList();
            }
        }

        private ProductCategory ToModel(ProductCategoryEntity entity)
        {
            if (entity == null) return null;
        
            return new ProductCategory()
            {
                Id = entity.Id,
                LocalizationToken = entity.LocalizationToken,
                Timestamp = entity.Timestamp,
                ParentId = entity.ParentId,
                Parent = ToModel(entity.Parent),
                IsLeaf = entity.Children.Count == 0,
            };
        }

        private ProductCategoryEntity ToEntity(ProductCategory category)
        {
            return new ProductCategoryEntity()
            {
                Id = category.Id,
                LocalizationToken = category.LocalizationToken,
                Timestamp = category.Timestamp,
                ParentId = category.ParentId,
            };
        }
    }
}