using System;
using System.Linq;

namespace MarginTrading.AssetService.Contracts.Extensions
{
    public static class TickFormulaExtensions
    {
        public static (decimal minPrice, decimal maxPrice, int precision) GetPrecisionInfo(this LegacyAsset.TickFormula formula,
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
        
        public static int GetPrecision(this decimal number)
        {
            return BitConverter.GetBytes(decimal.GetBits(number)[3])[2];
        }
    }
}