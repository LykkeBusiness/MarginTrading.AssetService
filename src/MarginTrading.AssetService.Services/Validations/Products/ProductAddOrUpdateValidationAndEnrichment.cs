using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services.Validations.Products
{
    [UsedImplicitly]
    public class
        ProductAddOrUpdateValidationAndEnrichment : ValidationAndEnrichmentChainEngine<Product, ProductsErrorCodes>
    {
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ICurrenciesService _currenciesService;
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoriesService _productCategoriesService;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IAssetTypesRepository _assetTypesRepository;

        public ProductAddOrUpdateValidationAndEnrichment(
            IUnderlyingsCache underlyingsCache,
            ICurrenciesService currenciesService,
            IMarketSettingsRepository marketSettingsRepository,
            IProductsRepository productsRepository,
            IProductCategoriesService productCategoriesService,
            ITickFormulaRepository tickFormulaRepository,
            IAssetTypesRepository assetTypesRepository)
        {
            _underlyingsCache = underlyingsCache;
            _currenciesService = currenciesService;
            _marketSettingsRepository = marketSettingsRepository;
            _productsRepository = productsRepository;
            _productCategoriesService = productCategoriesService;
            _tickFormulaRepository = tickFormulaRepository;
            _assetTypesRepository = assetTypesRepository;

            AddValidation(UnderlyingMustExist);
            AddValidation(CurrencyMustExist);
            AddValidation(MarketSettingsMustExist);
            AddValidation(TickFormulaMustExist);
            AddValidation(AssetTypeMustExist);
            AddValidation(SingleProductPerUnderlying);
            AddValidation(SetCategoryIdAsync);
            AddValidation(SetExistingFields);
        }

        private async Task<Result<Product, ProductsErrorCodes>> UnderlyingMustExist(Product value, string userName,
            string correlationId, Product existing = null)
        {
            var underlying = _underlyingsCache.GetByMdsCode(value.UnderlyingMdsCode);
            if (underlying == null)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);
            }

            value.TradingCurrency = underlying.TradingCurrency;
            // we use StartDate from the request, if possible, and fallback to the underlying's StartDate otherwise
            var startDate = value.StartDate ?? underlying.StartDate;
            if(existing != null && existing.IsStarted && startDate > DateTime.UtcNow) 
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotChangeStartDateFromPastToFuture);
                
            value.StartDate = startDate;
            value.IsStarted = startDate < DateTime.UtcNow;

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> CurrencyMustExist(Product value, string userName,
            string correlationId, Product existing = null)
        {
            var currencyResult = await _currenciesService.GetByIdAsync(value.TradingCurrency);
            if (currencyResult.IsFailed)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CurrencyDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> MarketSettingsMustExist(Product value, string userName,
            string correlationId, Product existing = null)
        {
            if (!await _marketSettingsRepository.ExistsAsync(value.Market))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.MarketSettingsDoNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> SingleProductPerUnderlying(Product value,
            string userName,
            string correlationId, Product existing = null)
        {
            var singleProductPerUnderlying =
                await _productsRepository.UnderlyingHasOnlyOneProduct(value.UnderlyingMdsCode, value.ProductId);
            if (!singleProductPerUnderlying)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CanOnlyCreateOneProductPerUnderlying);

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> SetCategoryIdAsync(Product value,
            string userName,
            string correlationId, Product existing = null)
        {
            var categoryResult = await _productCategoriesService.GetOrCreate(value.Category, userName, correlationId);
            if (categoryResult.IsFailed)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotCreateCategory);
            }

            var category = categoryResult.Value;
            if (!category.IsLeaf)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotCreateProductInNonLeafCategory);

            value.Category = category.Id;
            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> TickFormulaMustExist(Product value,
            string userName,
            string correlationId, Product existing = null)
        {
            if (!await _tickFormulaRepository.ExistsAsync(value.TickFormula))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.TickFormulaDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> AssetTypeMustExist(Product value,
            string userName,
            string correlationId, Product existing = null)
        {
            if (!await _assetTypesRepository.ExistsAsync(value.AssetType))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.AssetTypeDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> SetExistingFields(Product value,
            string userName,
            string correlationId, Product existing = null)
        {
            if (existing != null)
            {
                value.Timestamp = existing.Timestamp;
                value.IsFrozen = existing.IsFrozen;
                value.IsSuspended = existing.IsSuspended;
                value.IsDiscontinued = existing.IsDiscontinued;
                value.FreezeInfo = existing.FreezeInfo;
            }
            else
            {
                value.FreezeInfo = new ProductFreezeInfo();
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }
    }
}