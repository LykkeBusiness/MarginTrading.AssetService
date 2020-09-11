using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Services
{
    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;

        public ProductCategoriesService(IProductCategoriesRepository productCategoriesRepository,
            IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService)
        {
            _productCategoriesRepository = productCategoriesRepository;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
        }

        public async Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetOrCreate(string category,
            string username,
            string correlationId)
        {
            var categoryName = new ProductCategoryName(category);

            // early exit if the category already exists
            var existingCategory = await GetByIdAsync(categoryName.NormalizedName);
            if (existingCategory.IsSuccess) return existingCategory;

            // check all the nodes in the hierarchy and create missing ones
            var categories = categoryName.Nodes;
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
                    await PublishProductCategoryChangedEvent(null, productCategory, username, correlationId,
                        ChangeType.Creation, categoryName.GetOriginalNodeName(productCategory.Id));
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
                    await PublishProductCategoryChangedEvent(category.Value, null, username, correlationId,
                        ChangeType.Deletion);
                }
            }

            return category.ToResultWithoutValue();
        }


        public Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync()
            => _productCategoriesRepository.GetAllAsync();

        public Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id)
            => _productCategoriesRepository.GetByIdAsync(id);

        private async Task PublishProductCategoryChangedEvent(ProductCategory oldCategory, ProductCategory newCategory,
            string username, string correlationId, ChangeType changeType, string originalCategoryName = null)
        {
            await _cqrsMessageSender.SendEvent(new ProductCategoryChangedEvent()
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldProductCategory = _convertService.Convert<ProductCategory, ProductCategoryContract>(oldCategory),
                NewProductCategory = _convertService.Convert<ProductCategory, ProductCategoryContract>(newCategory),
                OriginalCategoryName = originalCategoryName,
            });
        }
    }
}