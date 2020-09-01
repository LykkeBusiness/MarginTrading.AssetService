using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;
        private readonly IUnderlyingsApi _underlyingsApi;

        public ProductsService(IProductsRepository repository, IAuditService auditService, IUnderlyingsApi underlyingsApi)
        {
            _repository = repository;
            _auditService = auditService;
            _underlyingsApi = underlyingsApi;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username, string correlationId)
        {
            var underlyingResponse = await _underlyingsApi.GetByIdAsync(product.UnderlyingMdsCode);
            if (underlyingResponse.ErrorCode == UnderlyingsErrorCodesContract.DoesNotExist)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);
            }
            
            var result = await _repository.InsertAsync(product);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                    product.ToJson());
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username, string correlationId)
        {
            var underlyingResponse = await _underlyingsApi.GetByIdAsync(product.UnderlyingMdsCode);
            if (underlyingResponse.ErrorCode == UnderlyingsErrorCodesContract.DoesNotExist)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);
            }
            
            var existing = await _repository.GetByIdAsync(product.ProductId);

            if (existing.IsSuccess)
            {
                var result = await _repository.UpdateAsync(product);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existing.Value.ToJson());    
                }

                return result;                
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, string username, string correlationId)
        {
            var existing = await _repository.GetByIdAsync(productId);

            if (existing.IsSuccess)
            {
                var result = await _repository.DeleteAsync(productId);

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
    }
}