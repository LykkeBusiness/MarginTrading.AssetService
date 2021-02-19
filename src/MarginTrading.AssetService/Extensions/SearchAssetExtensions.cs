using System;
using System.Collections.Generic;
using System.Globalization;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Services.Extensions;

namespace MarginTrading.AssetService.Extensions
{
    public static class SearchAssetExtensions
    {
        public static IEnumerable<Asset> Search(this ILegacyAssetsCache cache, SearchLegacyAssetsRequest request)
        {
            return cache.GetByFilter(x =>
            {
                //we have to provide format because expire date is not in default one
                var expiryDate = DateTime.TryParseExact(x.ExpiryDate, 
                    LegacyAssetExtensions.DateFormat,
                    DateTimeFormatInfo.InvariantInfo, 
                    DateTimeStyles.None, 
                    out var ed) ? ed : (DateTime?) null;

                return (string.IsNullOrWhiteSpace(request.UnderlyingIsIn) ||
                        (x.Underlying?.Isin.Contains(request.UnderlyingIsIn, StringComparison.InvariantCultureIgnoreCase) ?? false))
                       && (string.IsNullOrWhiteSpace(request.UnderlyingType) ||
                           (x.Underlying?.AssetType.Contains(request.UnderlyingType, StringComparison.InvariantCultureIgnoreCase) ?? false))
                       && (request.ExpiryDateFrom == null || expiryDate > request.ExpiryDateFrom)
                       && (request.ExpiryDateTo == null || expiryDate < request.ExpiryDateTo)
                       && (string.IsNullOrWhiteSpace(request.MdsCode) || x.UnderlyingMdsCode.Contains(request.MdsCode, StringComparison.InvariantCultureIgnoreCase))
                       && (string.IsNullOrWhiteSpace(request.AssetName) ||
                           x.Name.Contains(request.AssetName, StringComparison.InvariantCultureIgnoreCase));
            });
        }
    }
}
