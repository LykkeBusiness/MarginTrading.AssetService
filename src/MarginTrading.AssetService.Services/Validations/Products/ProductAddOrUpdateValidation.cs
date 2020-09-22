using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services.Validations.Products
{
    public class ProductAddOrUpdateValidation
    {
        private readonly ValidationChainEngine<Product, ProductsErrorCodes> _engine =
            new ValidationChainEngine<Product, ProductsErrorCodes>();

        private readonly IUnderlyingsApi _underlyingsApi;
        private readonly ICurrenciesService _currenciesService;
        private readonly IMarketSettingsRepository _marketSettingsRepository;
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoriesService _productCategoriesService;
        private readonly ITickFormulaRepository _tickFormulaRepository;
        private readonly IAssetTypesRepository _assetTypesRepository;

        public ProductAddOrUpdateValidation(IUnderlyingsApi underlyingsApi,
            ICurrenciesService currenciesService,
            IMarketSettingsRepository marketSettingsRepository,
            IProductsRepository productsRepository,
            IProductCategoriesService productCategoriesService,
            ITickFormulaRepository tickFormulaRepository,
            IAssetTypesRepository assetTypesRepository)
        {
            _underlyingsApi = underlyingsApi;
            _currenciesService = currenciesService;
            _marketSettingsRepository = marketSettingsRepository;
            _productsRepository = productsRepository;
            _productCategoriesService = productCategoriesService;
            _tickFormulaRepository = tickFormulaRepository;
            _assetTypesRepository = assetTypesRepository;

            _engine.AddValidation(UnderlyingMustExist);
            _engine.AddValidation(CurrencyMustExist);
            _engine.AddValidation(MarketSettingsMustExist);
            _engine.AddValidation(TickFormulaMustExist);
            _engine.AddValidation(AssetTypeMustExist);
            _engine.AddValidation(SingleProductPerUnderlying);
            _engine.AddValidation(SetCategoryIdAsync);
            _engine.AddValidation(SetExistingFields);
        }

        public Task<Result<Product, ProductsErrorCodes>> ValidateAllAsync(Product value, string userName,
            string correlationId, Product existing = null)
            => _engine.ValidateAllAsync(value, userName, correlationId, existing);

        private async Task<Result<Product, ProductsErrorCodes>> UnderlyingMustExist(Product value, string userName,
            string correlationId, Product existing = null)
        {
            var underlyingResponse = await _underlyingsApi.GetByIdAsync(value.UnderlyingMdsCode);
            if (underlyingResponse.ErrorCode == UnderlyingsErrorCodesContract.DoesNotExist)
            {
                return new Result<Product, ProductsErrorCodes>(ProductsErrorCodes.UnderlyingDoesNotExist);
            }

            value.TradingCurrency = underlyingResponse.Underlying.TradingCurrency;

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