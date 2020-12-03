using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using MarginTrading.AssetService.Core.Caches;
using Microsoft.Extensions.Caching.Memory;

namespace MarginTrading.AssetService.Services.Caches
{
    public class UnderlyingCategoriesCache : IUnderlyingCategoriesCache
    {
        private const int ExpirationInSeconds = 300;

        private readonly IUnderlyingCategoriesApi _underlyingCategoriesApi;
        private readonly IMemoryCache _cache;

        public UnderlyingCategoriesCache(IUnderlyingCategoriesApi underlyingCategoriesApi,
            IMemoryCache cache)
        {
            _underlyingCategoriesApi = underlyingCategoriesApi;
            _cache = cache;
        }

        public Task<List<UnderlyingCategoryCacheModel>> Get()
        {
            return _cache.GetOrCreateAsync("underlying_categories", async entity =>
            {
                entity.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ExpirationInSeconds);

                var locales = (await _underlyingCategoriesApi.GetAllAsync()).UnderlyingCategories
                    .Select(x => UnderlyingCategoryCacheModel.Create(x.Id, x.FinancingFeesFormula));

                return locales.ToList();
            });
        }
    }
}