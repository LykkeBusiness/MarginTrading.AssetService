using System;
using Lykke.Snow.Common.Percents;

namespace MarginTrading.AssetService.Core.Domain
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Margin rate as percent
        /// </summary>
        /// <param name="product"></param>
        /// <param name="profileMargin"></param>
        /// <returns></returns>
        //see https://lykke-snow.atlassian.net/browse/BUGS-1981 for details
        public static decimal GetMarginRateAsPercent(this Product product, decimal profileMargin)
        {
            if (product.Margin == null)
            {
                return profileMargin;
            }

            if (product.EnforceMargin)
            {
                return product.Margin.Value;
            }

            return Math.Max(profileMargin, product.Margin.Value);
        }

        /// <summary>
        /// Margin rate
        /// </summary>
        /// <param name="product"></param>
        /// <param name="profileMargin"></param>
        /// <returns></returns>
        public static MarginRate GetMarginRate(this Product product, decimal profileMargin) =>
            new MarginRate(product.GetMarginRateAsPercent(profileMargin));
    }
}