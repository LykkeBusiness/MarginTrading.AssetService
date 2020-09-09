using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using AuditDataType = MarginTrading.AssetService.Core.Domain.AuditDataType;

namespace MarginTrading.AssetService.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;
        private readonly IUnderlyingsApi _underlyingsApi;
        private readonly IProductCategoriesService _productCategoriesService;

        public ProductsService(IProductsRepository repository,
            IAuditService auditService,
            IUnderlyingsApi underlyingsApi,
            IProductCategoriesService productCategoriesService)
        {
            _repository = repository;
            _auditService = auditService;
            _underlyingsApi = underlyingsApi;
            _productCategoriesService = productCategoriesService;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username,
            string correlationId)
        {
            // underlyings check
            var underlyingExistsResult = await UnderlyingExists(product);
            if (underlyingExistsResult.IsFailed) return underlyingExistsResult;

            // categories check
            var productWithCategoryResult = await SetCategoryId(product, username, correlationId);
            if (productWithCategoryResult.IsFailed) return productWithCategoryResult.ToResultWithoutValue();
            
            var result = await _repository.InsertAsync(productWithCategoryResult.Value);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                    productWithCategoryResult.Value.ToJson());
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username,
            string correlationId)
        {
            // underlyings check
            var underlyingExistsResult = await UnderlyingExists(product);
            if (underlyingExistsResult.IsFailed) return underlyingExistsResult;

            // categories check
            var productWithCategoryResult = await SetCategoryId(product, username, correlationId);
            if (productWithCategoryResult.IsFailed) return productWithCategoryResult.ToResultWithoutValue();
            
            var existing = await _repository.GetByIdAsync(product.ProductId);

            if (existing.IsSuccess)
            {
                productWithCategoryResult.Value.Timestamp = existing.Value.Timestamp;
                var result = await _repository.UpdateAsync(productWithCategoryResult.Value);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        productWithCategoryResult.Value.ToJson(), existing.Value.ToJson());
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, string username,
            string correlationId)
        {
            var existing = await _repository.GetByIdAsync(productId);

            if (existing.IsSuccess)
            {
                var result = await _repository.DeleteAsync(productId, existing.Value.Timestamp);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, productId, AuditDataType.Product,
                        oldStateJson: existing.Value.ToJson());
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
            => _repository.GetByIdAsync(productId);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(int skip = default, int take = 20)
            => _repository.GetByPageAsync(skip, take);

        private async Task<Result<ProductsErrorCodes>> UnderlyingExists(Product product)
        {
            var underlyingResponse = await _underlyingsApi.GetByIdAsync(product.UnderlyingMdsCode);
            return underlyingResponse.ErrorCode == UnderlyingsErrorCodesContract.DoesNotExist
                ? new Result<ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist)
                : new Result<ProductsErrorCodes>();
        }

        private async Task<Result<Product, ProductsErrorCodes>> SetCategoryId(Product product, string username, string correlationId)
        {
            var categoryResult = await _productCategoriesService.GetOrCreate(product.Category, username, correlationId);
            if (categoryResult.IsFailed)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotCreateCategory);
            }

            product.Category = categoryResult.Value.Id;
            return new Result<Product, ProductsErrorCodes>(product);
        }
    }
}