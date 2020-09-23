using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using CorporateActions.Contracts;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
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
        private readonly ProductAddOrUpdateValidation _addOrUpdateValidation;
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;
        private readonly ICorporateActionsApi _corporateActionsApi;
        private readonly ICqrsMessageSender _cqrsMessageSender;

        public ProductsService(
            ProductAddOrUpdateValidation addOrUpdateValidation,
            IProductsRepository repository,
            ICqrsMessageSender cqrsMessageSender,
            IAuditService auditService,
            ICorporateActionsApi corporateActionsApi)
        {
            _addOrUpdateValidation = addOrUpdateValidation;
            _repository = repository;
            _auditService = auditService;
            _corporateActionsApi = corporateActionsApi;
            _cqrsMessageSender = cqrsMessageSender;
        }


        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username,
            string correlationId)
        {
            var validationResult = await _addOrUpdateValidation.ValidateAllAsync(product, username, correlationId);
            if (validationResult.IsFailed) return validationResult.ToResultWithoutValue();

            product = validationResult.Value;

            var result = await _repository.InsertAsync(product);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                    product.ToJson());
                await _cqrsMessageSender.SendEntityCreatedEvent<Product, ProductContract, ProductChangedEvent>(product,
                    username, correlationId);
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username,
            string correlationId)
        {
            var existing = await _repository.GetByIdAsync(product.ProductId);

            if (existing.IsSuccess)
            {
                var validationResult =
                    await _addOrUpdateValidation.ValidateAllAsync(product, username, correlationId, existing.Value);
                if (validationResult.IsFailed) return validationResult.ToResultWithoutValue();

                product = validationResult.Value;

                var result = await _repository.UpdateAsync(product);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existing.Value.ToJson());
                    await _cqrsMessageSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                        existing.Value, product,
                        username, correlationId);
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
                    await _cqrsMessageSender.SendEntityDeletedEvent<Product, ProductContract, ProductChangedEvent>(
                        existing.Value,
                        username, correlationId);
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<ProductsErrorCodes>> ChangeFrozenStatus(string productId, bool isFrozen,
            bool forceFreezeIfAlreadyFrozen,
            ProductFreezeInfo freezeInfo, string userName, string correlationId)
        {
            var existing = await _repository.GetByIdAsync(productId);
            if (existing.IsFailed) return existing.ToResultWithoutValue();

            if (isFrozen == false)
            {
                freezeInfo = new ProductFreezeInfo();
            }

            // can only freeze already frozen products with force freeze
            if (isFrozen && existing.Value.IsFrozen && !forceFreezeIfAlreadyFrozen)
                return new Result<ProductsErrorCodes>();

            if (existing.Value.IsDiscontinued)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotFreezeDiscontinuedProduct);
            }

            if (!isFrozen && existing.Value.FreezeInfo?.Reason != ProductFreezeReason.CostAndChargesGeneration)
            {
                var validateCAFreezeResult = await ValidateCAFreezeState(existing.Value);
                if (validateCAFreezeResult.IsFailed) return validateCAFreezeResult;
            }

            var result =
                await _repository.ChangeFrozenStatus(productId, isFrozen, existing.Value.Timestamp, freezeInfo);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, userName, productId, AuditDataType.Product,
                    result.Value.ToJson(), existing.Value.ToJson());
                await _cqrsMessageSender.SendEntityEditedEvent<Product, ProductContract, ProductChangedEvent>(
                    existing.Value, result.Value, userName, correlationId);
            }

            return result.ToResultWithoutValue();
        }

        private async Task<Result<ProductsErrorCodes>> ValidateCAFreezeState(Product product)
        {
            if (product.FreezeInfo?.Reason == ProductFreezeReason.CorporateAction)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotManuallyUnfreezeCorporateActionsFreeze);
            }

            if (product.FreezeInfo != null && product.FreezeInfo.IsDefault)
            {
                var frozenAssetDict = await _corporateActionsApi.GetFrozenAssetInfo();
                if (frozenAssetDict.Keys.Contains(product.ProductId))
                {
                    return new Result<ProductsErrorCodes>(ProductsErrorCodes
                        .CannotManuallyUnfreezeCorporateActionsFreeze);
                }
            }

            return new Result<ProductsErrorCodes>();
        }

        public Task<Result<Product, ProductsErrorCodes>> GetByIdAsync(string productId)
            => _repository.GetByIdAsync(productId);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(int skip = default, int take = 20)
            => _repository.GetByPageAsync(skip, take);
    }
}