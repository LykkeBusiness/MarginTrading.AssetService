using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public static class ProductExtensions
    {
        //see https://lykke-snow.atlassian.net/browse/BUGS-1981 for details
        public static decimal GetMargin(this Product product, decimal profileMargin)
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

    }
}