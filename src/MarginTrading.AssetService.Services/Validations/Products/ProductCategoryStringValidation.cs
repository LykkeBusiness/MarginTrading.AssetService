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

        private async Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustNotBeNullOrEmpty(string value,
            string userName,
            string existing = null)
        {
            return string.IsNullOrWhiteSpace(value)
                ? new Result<string, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes.CategoryStringIsNotValid)
                : new Result<string, ProductCategoriesErrorCodes>(value);
        }

        private async Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustBeOriginalOrNormalized(
            string value,
            string userName,
            string existing = null)
        {
            var str = value.Trim().Trim('/');

            if (str.Contains('.') && str.Contains("/"))
            {
                return new Result<string, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                    .CategoryStringIsNotValid);
            }

            return new Result<string, ProductCategoriesErrorCodes>(str);
        }

        private async Task<Result<string, ProductCategoriesErrorCodes>> CategoryStringMustNotHaveEmptyNodes(
            string value, string username, string existing)
        {
            var separator = IsNormalized(value) ? '.' : '/';

            var nodes = value.Split(separator);
            foreach (var node in nodes)
            {
                var normalizedNode = node.Trim();
                if (string.IsNullOrEmpty(normalizedNode))
                    return new Result<string, ProductCategoriesErrorCodes>(ProductCategoriesErrorCodes
                        .CategoryStringIsNotValid);
            }

            return new Result<string, ProductCategoriesErrorCodes>(value);
        }

        private async Task<Result<string, ProductCategoriesErrorCodes>> NormalizeCategoryString(string value, string username,
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