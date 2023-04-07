using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class AssetPairService : IAssetPairService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICurrenciesRepository _currenciesRepository;
        private readonly ISettlementCurrencyService _settlementCurrencyService;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;

        public AssetPairService(
            IProductsRepository productsRepository,
            ICurrenciesRepository currenciesRepository,
            ISettlementCurrencyService settlementCurrencyService,
            DefaultLegalEntitySettings defaultLegalEntitySettings)
        {
            _productsRepository = productsRepository;
            _currenciesRepository = currenciesRepository;
            _settlementCurrencyService = settlementCurrencyService;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
        }

        public async Task<IAssetPair> GetByIdAsync(string assetPairId)
        {
            var result = await _productsRepository.GetByIdAsync(assetPairId);

            return result.IsSuccess ? AssetPair.CreateFromProduct(result.Value, _defaultLegalEntitySettings.DefaultLegalEntity) : null;
        }

        public async Task<IReadOnlyList<IAssetPair>> GetAllIncludingFxParisWithFilterAsync(IEnumerable<string> 
        assetPairIds = null, bool onlyStarted = true)
        {
            var settlementCurrency = await _settlementCurrencyService.GetSettlementCurrencyAsync();
            var products = await _productsRepository.GetByProductsIdsAsync(assetPairIds);
            var currencies = await _currenciesRepository.GetAllAsync();

            var assetPairs = products
                .Where(x => !onlyStarted || x.IsStarted)
                .Select(x => AssetPair.CreateFromProduct(x, _defaultLegalEntitySettings.DefaultLegalEntity)).ToList();
            
            assetPairs.AddRange(currencies.Value
                .Where(x => !x.Id.Equals(settlementCurrency,
                    StringComparison.InvariantCultureIgnoreCase)).Select(x =>
                    AssetPair.CreateFromCurrency(x, _defaultLegalEntitySettings.DefaultLegalEntity, settlementCurrency)));

            return assetPairs;
        }
    }
}