using System.Threading.Tasks;
using MarginTrading.AssetService.Services.Validations.Products;
using Xunit;

namespace MarginTrading.AssetService.Tests.UnitTests
{
    public class ProductCategoryStringValidationTests
    {
        [Theory]
        [InlineData("stocks", "stocks")]
        [InlineData("/stocks/", "stocks")]
        [InlineData(" /stocks/ ", "stocks")]
        [InlineData("stocks/Germany/Dax 30", "stocks.germany.dax_30")]
        [InlineData("stocks.germany.dax_30", "stocks.germany.dax_30")]
        public async Task ValidCategory_ShouldSucceed(string category, string expected)
        {
            var validator = new ProductCategoryStringValidation();
            var result = await validator.ValidateAllAsync(category);

            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("stocks//Germany")]
        [InlineData("stocks/ /Germany")]
        [InlineData("stocks..germany")]
        [InlineData("stocks/germany.dax_30")]
        public async Task InvalidCategory_ShouldFail(string category)
        {
            var validator = new ProductCategoryStringValidation();
            var result = await validator.ValidateAllAsync(category);

            Assert.True(result.IsFailed);
        }
    }
}