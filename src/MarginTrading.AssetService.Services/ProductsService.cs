using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Validations.Products;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using AuditDataType = MarginTrading.AssetService.Core.Domain.AuditDataType;

namespace MarginTrading.AssetService.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ProductAddOrUpdateValidationAndEnrichment _addOrUpdateValidationAndEnrichment;
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;
        private readonly ILog _log;
        private readonly ICqrsEntityChangedSender _entityChangedSender;
        private readonly CorrelationContextAccessor _correlationContextAccessor;
        private readonly IIdentityGenerator _identityGenerator;

        public ProductsService(
            ProductAddOrUpdateValidationAndEnrichment addOrUpdateValidationAndEnrichment,
            IProductsRepository repository,
            ICqrsEntityChangedSender entityChangedSender,
            IAuditService auditService,
            ILog log,
            CorrelationContextAccessor correlationContextAccessor,
            IIdentityGenerator identityGenerator)
        {
            _addOrUpdateValidationAndEnrichment = addOrUpdateValidationAndEnrichment;
            _repository = repository;
            _auditService = auditService;
            _log = log;
            _entityChangedSender = entityChangedSender;
            _correlationContextAccessor = correlationContextAccessor;
            _identityGenerator = identityGenerator;
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
                var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                    _identityGenerator.GenerateId();
                await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                    product.ToJson());
                await _entityChangedSender.SendEntityCreatedEvent<Product, ProductContract, ProductChangedEvent>(
                    product,
                    username, correlationId);
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
                    var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                        _identityGenerator.GenerateId();
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existing.Value.ToJson());
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                        existing.Value, product,
                        username, correlationId);
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
                    var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                        _identityGenerator.GenerateId();
                    await _auditService.TryAudit(correlationId, username, productId, AuditDataType.Product,
                        oldStateJson: existing.Value.ToJson());
                    await _entityChangedSender.SendEntityDeletedEvent<Product, ProductContract, ProductChangedEvent>(
                        existing.Value,
                        username, correlationId);
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
                var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                    _identityGenerator.GenerateId();
                await _auditService.TryAudit(correlationId, userName, productId, AuditDataType.Product,
                    result.Value.ToJson(), existing.Value.ToJson());
                await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                    existing.Value, result.Value, userName, correlationId);
            }

            return result;
        }

        public Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
            => _repository.GetByIdAsync(productId);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(string[] mdsCodes, string[] productIds,
            bool? isStarted)
            => _repository.GetAllAsync(mdsCodes, productIds, isStarted);

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

                    var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                        _identityGenerator.GenerateId();
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existingProduct.ToJson());
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                        existingProduct, product,
                        username, correlationId);
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
                    var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                        _identityGenerator.GenerateId();
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        oldStateJson: existing.Value.ToJson());
                    await _entityChangedSender.SendEntityDeletedEvent<Product, ProductContract, ProductChangedEvent>(
                        product,
                        username, correlationId);
                }
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> DiscontinueBatchAsync(string[] productIds, string username)
        {
            if (!productIds.Any())
            {
                _log.WriteWarning(nameof(ProductsService), nameof(DiscontinueBatchAsync),
                    "Received empty list of product ids to discontinue");
                return new Result<ProductsErrorCodes>();
            }

            _log.WriteInfo(nameof(ProductsService), nameof(DiscontinueBatchAsync), $"Trying to discontinue products: {string.Join(',', productIds)}");
            var existing = (await _repository.GetAllAsync(null, productIds)).Value.ToHashSet();
            var updated = new HashSet<Product>();

            foreach (var productId in productIds)
            {
                var existingProduct = existing.FirstOrDefault(p => p.ProductId == productId);
                if (existingProduct == null)
                {
                    _log.WriteError(nameof(ProductsService), nameof(DiscontinueBatchAsync), new Exception($"Product to discontinue is not found: {productId}"));
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

                var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                    _identityGenerator.GenerateId();
                await _auditService.TryAudit(correlationId, username, updatedProduct.ProductId, AuditDataType.Product,
                    updatedProduct.ToJson(), existingProduct.ToJson());
                await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                    existingProduct, updatedProduct,
                    username, correlationId);
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
                    var correlationId = _correlationContextAccessor.CorrelationContext?.CorrelationId ??
                                        _identityGenerator.GenerateId();
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existing.Value.ToJson());
                    await _entityChangedSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                        oldProduct, product,
                        username, correlationId);
                }
            }

            return new Result<ProductsErrorCodes>();
        }
    }
}