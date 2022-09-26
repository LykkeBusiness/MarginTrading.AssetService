using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Helpers;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Services
{
    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly IProductCategoryStringService _productCategoryStringService;
        private readonly IAuditService _auditService;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly IConvertService _convertService;
        private readonly CorrelationContextAccessor _correlationContextAccessor;

        public ProductCategoriesService(IProductCategoriesRepository productCategoriesRepository,
            IProductCategoryStringService productCategoryStringService,
            IAuditService auditService,
            ICqrsMessageSender cqrsMessageSender,
            IConvertService convertService,
            CorrelationContextAccessor correlationContextAccessor)
        {
            _productCategoriesRepository = productCategoriesRepository;
            _productCategoryStringService = productCategoryStringService;
            _auditService = auditService;
            _cqrsMessageSender = cqrsMessageSender;
            _convertService = convertService;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetOrCreate(string category, string username)
        {
            var categoryNameResult = await _productCategoryStringService.Create(category);
            if (categoryNameResult.IsFailed)
                return new Result<ProductCategory, ProductCategoriesErrorCodes>(categoryNameResult.Error!.Value);

            var categoryName = categoryNameResult.Value;

            // early exit if the category already exists
            var existingCategory = await GetByIdAsync(categoryName.NormalizedName);
            if (existingCategory.IsSuccess) 
                return existingCategory;

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
                        return new Result<ProductCategory, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes.ParentHasAttachedProducts);
                }

                var result = await _productCategoriesRepository.InsertAsync(productCategory);
                if (!result.IsSuccess)
                    continue;
                
                await _auditService.CreateAuditRecord(AuditEventType.Creation, username, productCategory);

                await PublishProductCategoryChangedEvent(null, productCategory, username, ChangeType.Creation, categoryName.GetOriginalNodeName(productCategory.Id));
            }

            // return newly created leaf
            var newLeaf = await GetByIdAsync(categoryName.NormalizedName);
            return newLeaf;
        }

        public async Task<Result<ProductCategoriesErrorCodes>> DeleteAsync(string id, string username)
        {
            var category = await _productCategoriesRepository.GetByIdAsync(id);

            if (!category.IsSuccess) 
                return category.ToResultWithoutValue();
            
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
            if (!deleteResult.IsSuccess)
                return category.ToResultWithoutValue();
                
            await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, category.Value);
                    
            await PublishProductCategoryChangedEvent(category.Value, null, username, ChangeType.Deletion);

            return category.ToResultWithoutValue();
        }


        public Task<Result<List<ProductCategory>, ProductCategoriesErrorCodes>> GetAllAsync()
            => _productCategoriesRepository.GetAllAsync();

        public Task<Result<ProductCategory, ProductCategoriesErrorCodes>> GetByIdAsync(string id)
            => _productCategoriesRepository.GetByIdAsync(id);

        public async Task<List<string>> Validate(List<ProductAndCategoryPair> pairs)
        {
            var categoryNames = pairs.Select(x => x.Category).Distinct();

            var categoryNameValidations =
                new Dictionary<string, Result<ProductCategoryName, ProductCategoriesErrorCodes>>();
            foreach (var category in categoryNames)
            {
                var validationResult = await _productCategoryStringService.Create(category);
                categoryNameValidations.Add(category, validationResult);
            }

            // product category name validation failed
            if (categoryNameValidations.Any(kvp => kvp.Value.IsFailed))
            {
                return categoryNameValidations
                    .Where(kvp => kvp.Value.IsFailed)
                    .Select(kvp => $"Category \"{kvp.Key}\" is not a valid category")
                    .ToList();
            }

            var allNodes = categoryNameValidations
                .SelectMany(kvp => kvp.Value.Value.Nodes)
                .ToList();

            var leafHelper = new CategoryLeafHelper(allNodes);

            // product must be in a leaf category validation

            return (from pair in pairs
                    let categoryName = pair.Category
                    let normalizedCategoryName = categoryNameValidations[categoryName].Value.NormalizedName
                    where !leafHelper.IsLeaf(normalizedCategoryName)
                    select $"Product {pair.ProductId} cannot be attached to a category {categoryName} because it is not a leaf category")
                .ToList();
        }

        private async Task PublishProductCategoryChangedEvent(ProductCategory oldCategory, ProductCategory newCategory,
            string username, ChangeType changeType, string originalCategoryName = null)
        {
            await _cqrsMessageSender.SendEvent(new ProductCategoryChangedEvent
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = _correlationContextAccessor.GetOrGenerateCorrelationId(),
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<ProductCategory, ProductCategoryContract>(oldCategory),
                NewValue = _convertService.Convert<ProductCategory, ProductCategoryContract>(newCategory),
                OriginalCategoryName = originalCategoryName
            });
        }
    }
}