using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Validations.Products
{
    public class ProductCategoryStringValidation : ValidationAndEnrichmentChainEngine<string, ProductCategoriesErrorCodes>
    {
        public ProductCategoryStringValidation()
        {
            AddValidation(CategoryStringMustNotBeNullOrEmpty);
            AddValidation(CategoryStringMustBeOriginalOrNormalized);
            AddValidation(CategoryStringMustNotHaveEmptyNodes);
            AddValidation(NormalizeCategoryString);
        }

        private Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustNotBeNullOrEmpty(string value,
            string userName,
            string existing = null)
        {
            var result = string.IsNullOrWhiteSpace(value)
                ? ProductCategoriesErrorCodes.CategoryStringIsNotValid
                : new Result<string, ProductCategoriesErrorCodes>(value);

            return result;
        }

        private Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustBeOriginalOrNormalized(
            string value,
            string userName,
            string existing = null)
        {
            var str = value.Trim().Trim('/');

            if (str.Contains('.') && str.Contains("/"))
            {
                Result<string, ProductCategoriesErrorCodes> result = ProductCategoriesErrorCodes.CategoryStringIsNotValid;
                return result;
            }

            return new Result<string, ProductCategoriesErrorCodes>(str);
        }

        private Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustNotHaveEmptyNodes(
            string value, string username, string existing)
        {
            var separator = IsNormalized(value) ? '.' : '/';

            var nodes = value.Split(separator);
            foreach (var node in nodes)
            {
                var normalizedNode = node.Trim();
                if (string.IsNullOrEmpty(normalizedNode))
                {
                    Result<string, ProductCategoriesErrorCodes> result = ProductCategoriesErrorCodes.CategoryStringIsNotValid;
                    return result;
                }
            }

            return new Result<string, ProductCategoriesErrorCodes>(value);
        }

        private Task<Result<string, ProductCategoriesErrorCodes>> NormalizeCategoryString(string value, string username,
            string existing)
        {
            var normalized = IsNormalized(value)
                ? value
                : value.ToLower()
                    .Replace(' ', '_')
                    .Replace('/', '.');
            
            return new Result<string, ProductCategoriesErrorCodes>(normalized);
        }

        private bool IsNormalized(string value)
        {
            return value.Contains('.') && !value.Contains('/');
        }
    }
}