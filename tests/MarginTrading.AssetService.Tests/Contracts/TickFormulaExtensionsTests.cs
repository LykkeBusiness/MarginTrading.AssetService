using System.Collections.Generic;

using MarginTrading.AssetService.Contracts.Extensions;
using MarginTrading.AssetService.Contracts.LegacyAsset;

using Xunit;

namespace MarginTrading.AssetService.Tests.Contracts
{
    public class TickFormulaExtensionsTests
    {
        [Theory]
        [InlineData("1", 0)]
        [InlineData("1.0", 0)]
        [InlineData("1.00", 0)]
        [InlineData("1.10", 1)]
        [InlineData("1.100", 1)]
        [InlineData("1.010", 2)]
        [InlineData("1.0100", 2)]
        [InlineData("1.0010", 3)]
        [InlineData("1.00100", 3)]
        [InlineData("1.1", 1)]
        [InlineData("1.12", 2)]
        [InlineData("1.123", 3)]
        public void Test_Decimal_GetPrecision(string valueAsString, uint expectedPrecision)
        {
            // Act
            var value = decimal.Parse(valueAsString);
            var actualPrecision = value.GetPrecision();

            // Assert
            Assert.Equal(expectedPrecision, actualPrecision);
        }

        [Theory]
        [InlineData(0.001, 0.001, 0.01, 3)]
        [InlineData(0.0011, 0.001, 0.01, 3)]
        [InlineData(0.01, 0.01, 50, 2)]
        [InlineData(0.011, 0.01, 50, 2)]
        [InlineData(50, 50, 100, 2)]
        [InlineData(50.1, 50, 100, 2)]
        [InlineData(100, 100, 500, 1)]
        [InlineData(100.1, 100, 500, 1)]
        public void Test_TickFormula_GetPrecisionInfo(decimal price, decimal minPrice, decimal maxPrice, uint precision)
        {
            // Arrange
            var tickFormula = new TickFormula
            {
                Name = "testFormula",
                TickFormulaParameters = new TickFormulaParameters
                {
                    Ladders = new List<decimal> {0.001M, 0.01M, 50, 100, 500},
                    Values = new List<decimal> {0.001M, 0.01M, 0.05M, 0.1M, 0.5M}
                }
            };

            // Act
            var info = tickFormula.GetPrecisionInfo(price);

            // Assert
            Assert.Equal(minPrice, info.minPrice);
            Assert.Equal(maxPrice, info.maxPrice);
            Assert.Equal(precision, info.precision);
        }

        [Fact]
        public void Test_TickFormula_GetPrecisionInfo_Min()
        {
            // Arrange
            var tickFormula = new TickFormula
            {
                Name = "testFormula",
                TickFormulaParameters = new TickFormulaParameters
                {
                    Ladders = new List<decimal> {0.001M, 0.01M, 50, 100, 500},
                    Values = new List<decimal> {0.001M, 0.01M, 0.05M, 0.1M, 0.5M}
                }
            };

            // Act
            var info = tickFormula.GetPrecisionInfo(0.0001M);

            // Assert
            Assert.Equal(decimal.MinValue, info.minPrice);
            Assert.Equal(0.001M, info.maxPrice);
            Assert.Equal((uint)3, info.precision);
        }

        [Fact]
        public void Test_TickFormula_GetPrecisionInfo_Max()
        {
            // Arrange
            var tickFormula = new TickFormula
            {
                Name = "testFormula",
                TickFormulaParameters = new TickFormulaParameters
                {
                    Ladders = new List<decimal> {0.001M, 0.01M, 50, 100, 500},
                    Values = new List<decimal> {0.001M, 0.01M, 0.05M, 0.1M, 0.5M}
                }
            };

            // Act
            var info = tickFormula.GetPrecisionInfo(500);

            // Assert
            Assert.Equal(500, info.minPrice);
            Assert.Equal(decimal.MaxValue, info.maxPrice);
            Assert.Equal((uint)1, info.precision);
        }
    }
}