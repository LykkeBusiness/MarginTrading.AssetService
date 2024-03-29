using System;
using System.Linq;

namespace MarginTrading.AssetService.Contracts.Extensions
{
    public static class TickFormulaExtensions
    {
        public static (decimal minPrice, decimal maxPrice, uint precision) GetPrecisionInfo(this LegacyAsset.TickFormula formula,
            decimal price)
        {
            var minValue = decimal.MinValue;

            for (int i = 0; i < formula.TickFormulaParameters.Ladders.Count; i++)
            {
                var valueIndex = Math.Max(i - 1, 0);

                if (price < formula.TickFormulaParameters.Ladders[i])
                {
                    if (formula.TickFormulaParameters.Values.Count > valueIndex)
                    {
                        return (minValue, formula.TickFormulaParameters.Ladders[i], formula.TickFormulaParameters
                            .Values[valueIndex].GetPrecision());
                    }
                    else
                    {
                        return (minValue, formula.TickFormulaParameters.Ladders[i], formula.TickFormulaParameters
                            .Values.Last().GetPrecision());
                    }
                }

                minValue = formula.TickFormulaParameters.Ladders[i];
            }

            return (minValue, decimal.MaxValue, formula.TickFormulaParameters
                .Values.Last().GetPrecision());
        }
        
        public static uint GetPrecision(this decimal number)
        {
            if (number % 1 == 0)
            {
                return 0;
            }

            var normalized = RemoveTrailingZeros(number);
            return BitConverter.GetBytes(decimal.GetBits(normalized)[3])[2];
        }

        private static decimal RemoveTrailingZeros(decimal number)
        {
            return number / 1.000000000000000000000000000000000m;   
        }
    }
}