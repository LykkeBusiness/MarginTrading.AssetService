using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Audit;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Validations.Products;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ProductAddOrUpdateValidationAndEnrichment _addOrUpdateValidationAndEnrichment;
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;
        private readonly ILogger<ProductsService> _logger;
        private readonly ICqrsEntityChangedSender _entityChangedSender;

        public ProductsService(
            ProductAddOrUpdateValidationAndEnrichment addOrUpdateValidationAndEnrichment,
            IProductsRepository repository,
            ICqrsEntityChangedSender entityChangedSender,
            IAuditService auditService,
            ILogger<ProductsService> logger)
        {
            _addOrUpdateValidationAndEnrichment = addOrUpdateValidationAndEnrichment;
            _repository = repository;
            _auditService = auditService;
            _logger = logger;
            _entityChangedSender = entityChangedSender;
        }


        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username)
        {
            var validationResult =
                await _addOrUpdateValidationAndEnrichment.ValidateAllAsync(product, username);
            if (validationResult.IsFailed) return validationResult.ToResultWithoutValue();

            product = validationResult.Value;

            var result = await _repository.InsertAsync(product);

            if (result.IsSuccess)
            {
                await _auditService.CreateAuditRecord(AuditEventType.Creation, username, product);
                
                await _entityChangedSender.SendEntityCreatedEvent<Product, ProductContract, ProductChangedEvent>(product, username);
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username)
        {
            var existing = await _repository.GetByIdAsync(product.ProductId);

            if (existing.IsSuccess)
            {
                var validationResult =
                    await _addOrUpdateValidationAndEnrichment.ValidateAllAsync(product, username, existing.Value);
                if (validationResult.IsFailed) return validationResult.ToResultWithoutValue();

                product = validationResult.Value;

                var result = await _repository.UpdateAsync(product);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Edition, username, product, existing.Value);
                    
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(existing.Value, product, username);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<ProductsErrorCodes>> DeleteAsync(string productId, string username)
        {
            var existing = await _repository.GetByIdAsync(productId);

            if (existing.IsSuccess)
            {
                if (existing.Value.IsStarted)
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotDeleteStartedProduct);

                var result = await _repository.DeleteAsync(productId, existing.Value.Timestamp);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, existing.Value);
                    
                    await _entityChangedSender.SendEntityDeletedEvent<Product, ProductContract, ProductChangedEvent>(existing.Value, username);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<Product, ProductsErrorCodes>> ChangeFrozenStatus(string productId, bool isFrozen,
            bool forceFreezeIfAlreadyFrozen,
            ProductFreezeInfo freezeInfo, string userName)
        {
            var existing = await _repository.GetByIdAsync(productId);
            if (existing.IsFailed) return existing;

            if (isFrozen == false)
            {
                freezeInfo = new ProductFreezeInfo();
            }

            // can only freeze already frozen products with force freeze
            if (isFrozen && existing.Value.IsFrozen && !forceFreezeIfAlreadyFrozen)
                return existing;

            if (existing.Value.IsDiscontinued)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotFreezeDiscontinuedProduct);
            }

            var result =
                await _repository.ChangeFrozenStatus(productId, isFrozen, existing.Value.Timestamp, freezeInfo);

            if (result.IsSuccess)
            {
                await _auditService.CreateAuditRecord(AuditEventType.Edition, userName, result.Value, existing.Value);
                
                await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(existing.Value, result.Value, userName);
            }

            return result;
        }

        public Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
            => _repository.GetByIdAsync(productId);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted, bool? isDiscontinued)
            => _repository.GetAllAsync(mdsCodes, productIds, isStarted, isDiscontinued);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted,
            bool? isDiscontinued,
            int skip = default, int take = 20)
            => _repository.GetByPageAsync(mdsCodes, productIds, isStarted, isDiscontinued, skip, take);

        public async Task<Result<ProductsErrorCodes>> UpdateBatchAsync(List<Product> products, string username)
        {
            var existing = await _repository.GetAllAsync(null, products.Select(p => p.ProductId).ToArray());
            foreach (var product in products)
            {
                var existingProduct = existing.Value.FirstOrDefault(p => p.ProductId == product.ProductId);
                var validationResult =
                    await _addOrUpdateValidationAndEnrichment.ValidateAllAsync(product, username, existingProduct);

                if (validationResult.IsFailed) return validationResult.ToResultWithoutValue();
            }

            var result = await _repository.UpdateBatchAsync(products);

            if (result.IsSuccess)
            {
                foreach (var product in products)
                {
                    var existingProduct = existing.Value.FirstOrDefault(p => p.ProductId == product.ProductId);

                    await _auditService.CreateAuditRecord(AuditEventType.Edition, username, product, existingProduct);
                    
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(existingProduct, product, username);
                }
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> DeleteBatchAsync(List<string> productIds, string username)
        {
            var existing = await _repository.GetAllAsync(null, productIds.ToArray());

            if (existing.Value.Any(x => x.IsStarted))
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotDeleteStartedProduct);

            var withTimestamps = productIds.ToDictionary(productId => productId,
                productId => existing.Value.FirstOrDefault(p => p.ProductId == productId)?.Timestamp);

            var result = await _repository.DeleteBatchAsync(withTimestamps);

            if (result.IsSuccess)
            {
                foreach (var product in existing.Value)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, product);
                    
                    await _entityChangedSender.SendEntityDeletedEvent<Product, ProductContract, ProductChangedEvent>(product, username);
                }
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> DiscontinueBatchAsync(string[] productIds, string username)
        {
            if (!productIds.Any())
            {
                _logger.LogWarning("Received empty list of product ids to discontinue");
                return new Result<ProductsErrorCodes>();
            }

            _logger.LogInformation("Trying to discontinue products: {Products}", string.Join(',', productIds));
            var existing = (await _repository.GetAllAsync(null, productIds)).Value.ToHashSet();
            var updated = new HashSet<Product>();

            foreach (var productId in productIds)
            {
                var existingProduct = existing.FirstOrDefault(p => p.ProductId == productId);
                if (existingProduct == null)
                {
                    _logger.LogError("Product to discontinue is not found: {ProductId}", productId);
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes.DoesNotExist);
                }

                if (existingProduct.IsDiscontinued)
                    continue;

                var productToUpdate = existingProduct.ShallowCopy();
                productToUpdate.IsDiscontinued = true;

                updated.Add(productToUpdate);
            }

            var result = await _repository.UpdateBatchAsync(updated.ToList());

            if (!result.IsSuccess)
                return result;

            foreach (var updatedProduct in updated)
            {
                var existingProduct = existing.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);

                await _auditService.CreateAuditRecord(AuditEventType.Edition, username, updatedProduct, existingProduct);
                
                await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(existingProduct, updatedProduct, username);
            }

            return result;
        }

        public Task<Result<ProductsCounter, ProductsErrorCodes>> GetAllCountAsync(string[] mdsCodes,
            string[] productIds)
            => _repository.GetAllCountAsync(mdsCodes, productIds);

        public async Task<Result<ProductsErrorCodes>> ChangeUnderlyingMdsCodeAsync(string oldMdsCode, string newMdsCode,
            string username)
        {
            var existing = await _repository.GetByUnderlyingMdsCodeAsync(oldMdsCode);

            foreach (var oldProduct in existing.Value)
            {
                var product = oldProduct.ShallowCopy();
                product.UnderlyingMdsCode = newMdsCode;

                var result = await _repository.UpdateAsync(product);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Edition, username, product, oldProduct);
                    
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(oldProduct, product, username);
                }
            }

            return new Result<ProductsErrorCodes>();
        }

        public async Task<Result<ProductsErrorCodes>> ChangeTradingDisabledAsync(string productId, bool tradingDisabled,
            string username)
        {
            var existing = await _repository.GetByIdAsync(productId);
            if (existing.IsFailed) return existing;

            var oldProduct = existing.Value;

            if (oldProduct.IsDiscontinued)
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotChangeDiscontinuedProduct);

            var product = oldProduct.ShallowCopy();
            product.IsTradingDisabled = tradingDisabled;

            var result = await _repository.UpdateAsync(product);

            if (result.IsSuccess)
            {
                await _auditService.CreateAuditRecord(AuditEventType.Edition, username, product, oldProduct);
                
                await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(oldProduct, product, username);
            }

            return result;
        }
    }
}