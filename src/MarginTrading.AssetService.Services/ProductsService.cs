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
        private readonly ICurrenciesService _currenciesService;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IAssetTypesRepository _assetTypesRepository;
        private readonly IMarketSettingsRepository _marketSettingsRepository;

        public ProductsService(IProductsRepository repository,
            IAuditService auditService,
            IUnderlyingsApi underlyingsApi,
            IProductCategoriesService productCategoriesService,
            IMarketSettingsRepository marketSettingsRepository,
            ICurrenciesService currenciesService,
            ITickFormulaRepository tickFormulaRepository,
            IAssetTypesRepository assetTypesRepository)
        {
            _repository = repository;
            _auditService = auditService;
            _underlyingsApi = underlyingsApi;
            _productCategoriesService = productCategoriesService;
            _marketSettingsRepository = marketSettingsRepository;
            _currenciesService = currenciesService;
            _tickFormulaRepository = tickFormulaRepository;
            _assetTypesRepository = assetTypesRepository;
        }

        public async Task<Result<ProductsErrorCodes>> InsertAsync(Product product, string username,
            string correlationId)
        {
            // underlyings check
            var underlyingExistsResult = await UnderlyingExistsAndSetTradingCurrencyAsync(product);
            if (underlyingExistsResult.IsFailed) return underlyingExistsResult;

            // currencies check
            var currencyExistsResult = await CurrencyExistsAsync(product);
            if (currencyExistsResult.IsFailed) return currencyExistsResult;
            
            // one product per underlying check
            var oneUnderlyingPerProduct =
                await _repository.UnderlyingHasOnlyOneProduct(product.UnderlyingMdsCode, product.ProductId);
            if(!oneUnderlyingPerProduct) return new Result<ProductsErrorCodes>(ProductsErrorCodes.CanOnlyCreateOneProductPerUnderlying);
            
            // categories check
            var productWithCategoryResult = await SetCategoryIdAsync(product, username, correlationId);
            if (productWithCategoryResult.IsFailed) return productWithCategoryResult;

            // market settings check
            if (!await _marketSettingsRepository.ExistsAsync(product.Market))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.MarketSettingsDoNotExist);
            }
            
            // tick formula check
            if (!await _tickFormulaRepository.ExistsAsync(product.TickFormula))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.TickFormulaDoesNotExist);
            }
            
            // asset type check
            if (!await _assetTypesRepository.ExistsAsync(product.AssetType))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.AssetTypeDoesNotExist);
            }

            var result = await _repository.InsertAsync(product);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, username, product.ProductId, AuditDataType.Product,
                    product.ToJson());
            }

            return result;
        }

        public async Task<Result<ProductsErrorCodes>> UpdateAsync(Product product, string username,
            string correlationId)
        {
            // underlyings check
            var underlyingExistsResult = await UnderlyingExistsAndSetTradingCurrencyAsync(product);
            if (underlyingExistsResult.IsFailed) return underlyingExistsResult;
            
            // currencies check
            var currencyExistsResult = await CurrencyExistsAsync(product);
            if (currencyExistsResult.IsFailed) return currencyExistsResult;
            
            // one product per underlying check
            var oneUnderlyingPerProduct =
                await _repository.UnderlyingHasOnlyOneProduct(product.UnderlyingMdsCode, product.ProductId);
            if(!oneUnderlyingPerProduct) return new Result<ProductsErrorCodes>(ProductsErrorCodes.CanOnlyCreateOneProductPerUnderlying);

            // categories check
            var productWithCategoryResult = await SetCategoryIdAsync(product, username, correlationId);
            if (productWithCategoryResult.IsFailed) return productWithCategoryResult;

            // market settings check
            if (!await _marketSettingsRepository.ExistsAsync(product.Market))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.MarketSettingsDoNotExist);
            }
            
            // tick formula check
            if (!await _tickFormulaRepository.ExistsAsync(product.TickFormula))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.TickFormulaDoesNotExist);
            }
            
            // asset type check
            if (!await _assetTypesRepository.ExistsAsync(product.AssetType))
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.AssetTypeDoesNotExist);
            }

            var existing = await _repository.GetByIdAsync(product.ProductId);

            if (existing.IsSuccess)
            {
                product.Timestamp = existing.Value.Timestamp;
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

        public Task<Result<List<Product>, ProductsErrorCodes>> GetAllAsync(string[] mdsCodes, string[] productIds)
            => _repository.GetAllAsync(mdsCodes, productIds);

        public Task<Result<List<Product>, ProductsErrorCodes>> GetByPageAsync(string[] mdsCodes, string[] productIds, int skip = default, int take = 20)
            => _repository.GetByPageAsync(mdsCodes, productIds, skip, take);

        private async Task<Result<ProductsErrorCodes>> UnderlyingExistsAndSetTradingCurrencyAsync(Product product)
        {
            var underlyingResponse = await _underlyingsApi.GetByIdAsync(product.UnderlyingMdsCode);
            if (underlyingResponse.ErrorCode == UnderlyingsErrorCodesContract.DoesNotExist)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);
            }

            product.TradingCurrency = underlyingResponse.Underlying.TradingCurrency;

            return new Result<ProductsErrorCodes>();
        }

        private async Task<Result<ProductsErrorCodes>> CurrencyExistsAsync(Product product)
        {
            var currencyResult = await _currenciesService.GetByIdAsync(product.TradingCurrency);
            if (currencyResult.IsFailed)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CurrencyDoesNotExist);
            }
            return new Result<ProductsErrorCodes>();
        }

        private async Task<Result<ProductsErrorCodes>> SetCategoryIdAsync(Product product, string username,
            string correlationId)
        {
            var categoryResult = await _productCategoriesService.GetOrCreate(product.Category, username, correlationId);
            if (categoryResult.IsFailed)
            {
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotCreateCategory);
            }

            var category = categoryResult.Value;
            if (!category.IsLeaf)
                return new Result<ProductsErrorCodes>(ProductsErrorCodes.CannotCreateProductInNonLeafCategory);

            product.Category = category.Id;
            return new Result<ProductsErrorCodes>();
        }
    }
}