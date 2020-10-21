using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Validations.Products;

namespace MarginTrading.AssetService.Services
{
    public class ProductCategoryStringService : IProductCategoryStringService
    {
        private readonly ProductCategoryStringValidation _validation;

        public ProductCategoryStringService(ProductCategoryStringValidation validation)
        {
            _validation = validation;
        }

        public async Task<Result<ProductCategoryName, ProductCategoriesErrorCodes>> Create(string category)
        {
            var validationResult = await _validation.ValidateAllAsync(category);

            if (validationResult.IsFailed)
                return new Result<ProductCategoryName, ProductCategoriesErrorCodes>(validationResult.Error.Value);

            var name = new ProductCategoryName(category, validationResult.Value);
            return new Result<ProductCategoryName, ProductCategoriesErrorCodes>(name);
        }
    }
}