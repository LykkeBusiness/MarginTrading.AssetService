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

        public async Task<Result<ProductCategoriesErrorCodes>> UpsertAsync(string category, string username,
            string correlationId)
        {
            var categories = ToCategories(category);
            var categoriesInDb = await _productCategoriesRepository.GetAllAsync();

            var categoriesToCreate = categories
                .Where(productCategory => categoriesInDb.Value.All(x => x.Id != productCategory.Id));

            foreach (var productCategory in categoriesToCreate)
            {
                // todo: check if parent category has products
                var result = await _productCategoriesRepository.InsertAsync(productCategory);
                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, productCategory.Id,
                        AuditDataType.ProductCategory,
                        productCategory.ToJson());
                }
            }

            return new Result<ProductCategoriesErrorCodes>();
        }

        public async Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, string username,
            string correlationId)
        {
            // todo: check if category has products
            var category = await _productCategoriesRepository.GetByIdAsync(id);

            if (category.IsSuccess)
            {
                if (!category.Value.IsLeaf)
                {
                    return new Result<ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                        .CannotDeleteNonLeafCategory);
                }


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

        private List<ProductCategory> ToCategories(string category)
        {
            var str = category
                .ToLower()
                .Replace(' ', '_')
                .Replace('/', '.');

            // assumes that category does not contain dots
            var arr = str.Split('.');

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