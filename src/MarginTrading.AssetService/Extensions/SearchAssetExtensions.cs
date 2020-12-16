using System;
using System.Collections.Generic;
using Cronut.Dto.Assets;
using MarginTrading.AssetService.Contracts.Asset;
using MarginTrading.AssetService.Core.Caches;

namespace MarginTrading.AssetService.Extensions
{
    public static class SearchAssetExtensions
    {
        public static IEnumerable<Asset> Search(this ILegacyAssetsCache cache, SearchLegacyAssetsRequest request)
        {
            return cache.GetByFilter(x =>
            {
                var expiryDate = DateTime.TryParse(x.ExpiryDate, out var ed) ? ed : (DateTime?) null;
                return (string.IsNullOrWhiteSpace(request.UnderlyingIsIn) ||
                        (x.Underlying?.Isin.Contains(request.UnderlyingIsIn, StringComparison.InvariantCultureIgnoreCase) ?? false))
                       && (string.IsNullOrWhiteSpace(request.UnderlyingType) ||
                           (x.Underlying?.AssetType.Contains(request.UnderlyingType, StringComparison.InvariantCultureIgnoreCase) ?? false))
                       && (expiryDate == null || request.ExpiryDateFrom == null || expiryDate > request.ExpiryDateFrom)
                       && (expiryDate == null || request.ExpiryDateTo == null || expiryDate < request.ExpiryDateTo)
                       && (string.IsNullOrWhiteSpace(request.MdsCode) || x.UnderlyingMdsCode.Contains(request.MdsCode, StringComparison.InvariantCultureIgnoreCase))
                       && (string.IsNullOrWhiteSpace(request.AssetName) ||
                           x.Name.Contains(request.AssetName, StringComparison.InvariantCultureIgnoreCase));
            });
        }
    }
}
