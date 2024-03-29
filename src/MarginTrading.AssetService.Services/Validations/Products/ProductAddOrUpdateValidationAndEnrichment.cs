using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Common.Model;
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
        private readonly IProductCategoriesService _productCategoriesService;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IAssetTypesRepository _assetTypesRepository;
        private readonly IProductsRepository _productsRepository;

        public ProductAddOrUpdateValidationAndEnrichment(
            IUnderlyingsCache underlyingsCache,
            ICurrenciesService currenciesService,
            IMarketSettingsRepository marketSettingsRepository,
            IProductCategoriesService productCategoriesService,
            ITickFormulaRepository tickFormulaRepository,
            IAssetTypesRepository assetTypesRepository,
            IProductsRepository productsRepository)
        {
            _underlyingsCache = underlyingsCache;
            _currenciesService = currenciesService;
            _marketSettingsRepository = marketSettingsRepository;
            _productCategoriesService = productCategoriesService;
            _tickFormulaRepository = tickFormulaRepository;
            _assetTypesRepository = assetTypesRepository;
            _productsRepository = productsRepository;

            AddValidation(UnderlyingMustExist);
            AddValidation(LongIsinMustBeUniqueAcrossAllIsins);
            AddValidation(ShortIsinMustBeUniqueAcrossAllIsins);
            AddValidation(CurrencyMustExist);
            AddValidation(MarketSettingsMustExist);
            AddValidation(TickFormulaMustExist);
            AddValidation(AssetTypeMustExist);
            AddValidation(SetCategoryIdAsync);
            AddValidation(SetExistingFields);
            AddValidation(ContractSizeCannotBeUpdatedForStarted);
        }

        private Task<Result<Product, ProductsErrorCodes>> ContractSizeCannotBeUpdatedForStarted(Product value, string userName,
            Product existing = null)
        {
            if (existing != null && existing.IsStarted && existing.ContractSize != value.ContractSize)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotUpdateContractSizeForStarted);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> LongIsinMustBeUniqueAcrossAllIsins(Product value, string userName,
            Product existing = null)
        {
            if (_underlyingsCache.IsinExists(value.IsinLong))
            {
                return ProductsErrorCodes.LongIsinNotUnique;
            }
            var exists = await _productsRepository.IsinExists(value.IsinLong, false);
            if (exists.result && exists.id != existing?.ProductId)
            {
                return ProductsErrorCodes.LongIsinNotUnique;
            }
            return value;
        }

        private async Task<Result<Product, ProductsErrorCodes>> ShortIsinMustBeUniqueAcrossAllIsins(Product value, string userName,
            Product existing = null)
        {
            if (value.IsinShort == value.IsinLong)
            {
                return ProductsErrorCodes.ShortIsinNotUnique;
            }
            if (_underlyingsCache.IsinExists(value.IsinShort))
            {
                return ProductsErrorCodes.ShortIsinNotUnique;
            }
            var exists = await _productsRepository.IsinExists(value.IsinShort, false);
            if (exists.result && exists.id != existing?.ProductId)
            {
                return ProductsErrorCodes.ShortIsinNotUnique;
            }
            return value;
        }

        private Task<Result<Product, ProductsErrorCodes>> UnderlyingMustExist(Product value, string userName,
            Product existing = null)
        {
            var underlying = _underlyingsCache.GetByMdsCode(value.UnderlyingMdsCode);
            if (underlying == null)
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);

            value.TradingCurrency = underlying.TradingCurrency;

            DateOnly? startDate;
            if (existing == null)
            {
                // we use StartDate from the request, if possible, and fallback to the underlying's StartDate otherwise
                startDate = value.StartDate ?? underlying.StartDate;
            }
            else
            {
                // for existing products we should not update StartDate from underlying 
                startDate = value.StartDate ?? existing.StartDate;
            }

            if (existing != null && existing.IsStarted && startDate > DateOnly.FromDateTime(DateTime.UtcNow))
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CannotChangeStartDateFromPastToFuture);

            value.StartDate = startDate;
            value.IsStarted = startDate < DateOnly.FromDateTime(DateTime.UtcNow);

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> CurrencyMustExist(Product value, string userName,
            Product existing = null)
        {
            var currencyResult = await _currenciesService.GetByIdAsync(value.TradingCurrency);
            if (currencyResult.IsFailed)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.CurrencyDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> MarketSettingsMustExist(Product value, string userName,
            Product existing = null)
        {
            if (!await _marketSettingsRepository.ExistsAsync(value.Market))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.MarketSettingsDoNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> SetCategoryIdAsync(Product value,
            string userName, Product existing = null)
        {
            var categoryResult = await _productCategoriesService.GetOrCreate(value.Category, userName);
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
            string userName, Product existing = null)
        {
            if (!await _tickFormulaRepository.ExistsAsync(value.TickFormula))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.TickFormulaDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private async Task<Result<Product, ProductsErrorCodes>> AssetTypeMustExist(Product value,
            string userName, Product existing = null)
        {
            if (!await _assetTypesRepository.ExistsAsync(value.AssetType))
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.AssetTypeDoesNotExist);
            }

            return new Result<Product, ProductsErrorCodes>(value);
        }

        private Task<Result<Product, ProductsErrorCodes>> SetExistingFields(Product value,
            string userName, Product existing = null)
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