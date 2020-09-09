using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Services
{
    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly IAuditService _auditService;

        public ProductCategoriesService(IProductCategoriesRepository productCategoriesRepository,
            IAuditService auditService)
        {
            _productCategoriesRepository = productCategoriesRepository;
            _auditService = auditService;
        }

        public async Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetOrCreate(string category,
            string username,
            string correlationId)
        {
            var categoryName = new ProductCategoryName(category);

            // early exit if the category leaf already exists
            var leaf = await GetByIdAsync(categoryName.NormalizedName);
            if (leaf.IsSuccess) return leaf;

            // check all the nodes in the hierarchy and create missing ones
            var categories = ToCategories(categoryName);
            var categoriesInDb = await _productCategoriesRepository.GetAllAsync();

            var categoriesToCreate = categories
                .Where(productCategory => categoriesInDb.Value.All(x => x.Id != productCategory.Id));

            foreach (var productCategory in categoriesToCreate)
            {
                // check if parent already has attached products - cannot create a leaf
                if (productCategory.ParentId != null)
                {
                    var parentHasProducts =
                        await _productCategoriesRepository.CategoryHasProducts(productCategory.ParentId);
                    if (parentHasProducts)
                        return new Result<ProductCategory, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                            .ParentHasAttachedProducts);
                }

                var result = await _productCategoriesRepository.InsertAsync(productCategory);
                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, productCategory.Id,
                        AuditDataType.ProductCategory,
                        productCategory.ToJson());
                }
            }

            // return newly created leaf
            var newLeaf = await GetByIdAsync(categoryName.NormalizedName);
            return newLeaf;
        }

        public async Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, string username,
            string correlationId)
        {
            var category = await _productCategoriesRepository.GetByIdAsync(id);

            if (category.IsSuccess)
            {
                // if category has children, deny the delete request
                if (!category.Value.IsLeaf)
                {
                    return new Result<ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                        .CannotDeleteNonLeafCategory);
                }

                // if category has attached products, deny the delete request
                var hasProducts = await _productCategoriesRepository.CategoryHasProducts(id);
                if (hasProducts)
                    return new Result<ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                        .CannotDeleteCategoryWithAttachedProducts);

                var deleteResult = await _productCategoriesRepository.DeleteAsync(id, category.Value.Timestamp);
                if (deleteResult.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, id, AuditDataType.ProductCategory,
                        oldStateJson: category.Value.ToJson());
                }
            }

            return category.ToResultWithoutValue();
        }


        public Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync()
            => _productCategoriesRepository.GetAllAsync();

        public Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id)
            => _productCategoriesRepository.GetByIdAsync(id);

        // todo: extract into a separate service & add tests
        private List<ProductCategory> ToCategories(ProductCategoryName categoryName)
        {
            // assumes that category does not contain dots
            var arr = categoryName.NormalizedName.Split('.');

            var result = new List<ProductCategory>();
            string fullPath = null;
            string parentId = null;

            for (var i = 0; i < arr.Length; i++)
            {
                fullPath = i == 0 ? arr[0] : string.Join('.', fullPath, arr[i]);
                var productCategory = new ProductCategory()
                {
                    Id = fullPath,
                    LocalizationToken = $"categoryName.{fullPath}",
                    ParentId = parentId,
                };
                result.Add(productCategory);
                parentId = fullPath;
            }

            return result;
        }
    }
}