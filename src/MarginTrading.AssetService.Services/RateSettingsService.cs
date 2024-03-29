﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain.Rates;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings.Rates;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;


namespace MarginTrading.AssetService.Services
{
    public class RateSettingsService : IRateSettingsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly DefaultRateSettings _defaultRateSettings;
        private readonly ILogger<RateSettingsService> _logger;

        public RateSettingsService(
            IProductsRepository productsRepository,
            IUnderlyingsCache underlyingsCache,
            ICurrenciesRepository currenciesRepository,
            DefaultRateSettings defaultRateSettings,
            ILogger<RateSettingsService> logger)
        {
            _productsRepository = productsRepository;
            _underlyingsCache = underlyingsCache;
            _currenciesRepository = currenciesRepository;
            _defaultRateSettings = defaultRateSettings;
            _logger = logger;
        }

        #region Overnight Swaps

        public async Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync(IList<string> assetPairIds = null)
        {
            var products = (await _productsRepository.GetByProductsIdsAsync(assetPairIds)).ToDictionary(x => x.ProductId, v=> v);

            //If filter is empty we should get all products(asset pairs)
            if (assetPairIds == null || !assetPairIds.Any())
                assetPairIds = products.Keys.ToList();
            
            var productTradingCurrencyMap = products.ToDictionary(x => x.Key, v => v.Value.TradingCurrency);

            var tradingCurrencies =
                (await _currenciesRepository.GetByIdsAsync(productTradingCurrencyMap.Values)).ToDictionary(x => x.Id, v => v);

            var underlyings = products.Select(x => x.Value.UnderlyingMdsCode).Distinct()
                .Select(_underlyingsCache.GetByMdsCode)
                .ToDictionary(x => x.MdsCode, v => v);

            var baseCurrenciesIds = underlyings.Values.Where(x => !string.IsNullOrEmpty(x.BaseCurrency))
                .Select(x => x.BaseCurrency).Distinct();

            var baseCurrencies =
                (await _currenciesRepository.GetByIdsAsync(baseCurrenciesIds)).ToDictionary(x => x.Id, v => v);

            var result = new List<OvernightSwapRate>();
            foreach (var assetPairId in assetPairIds)
            {
                products.TryGetValue(assetPairId, out var product);
                if (product == null)
                {
                    _logger.LogWarning("Missing product with id: {AssetPairId} , default values will be used to create OvernightSwapRate", assetPairId);
                    result.Add(OvernightSwapRate.FromDefault(_defaultRateSettings.DefaultOvernightSwapSettings, assetPairId));
                    continue;
                }

                var productId = product.ProductId;
                underlyings.TryGetValue(product.UnderlyingMdsCode, out var underlying);
                var baseCurrencyId = underlying?.BaseCurrency;
                var baseCurrency = string.IsNullOrEmpty(baseCurrencyId) ? null :
                    baseCurrencies.ContainsKey(baseCurrencyId) ? baseCurrencies[baseCurrencyId] : null;
                var tradingCurrencyId = productTradingCurrencyMap[productId];
                var tradingCurrency = tradingCurrencies[tradingCurrencyId];

                var rate = new OvernightSwapRate(productId,
                    underlying?.RepoSurchargePercent ??
                    _defaultRateSettings.DefaultOvernightSwapSettings.RepoSurchargePercent,
                    baseCurrency?.InterestRateMdsCode,
                    tradingCurrency.InterestRateMdsCode);

                result.Add(rate);
            }

            return result;
        }

        #endregion Overnight Swaps
    }
}