using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Common;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class TradingInstrumentsService : ITradingInstrumentsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly DefaultTradingInstrumentSettings _defaultTradingInstrumentSettings;

        public TradingInstrumentsService(
            IProductsRepository productsRepository,
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IUnderlyingsCache underlyingsCache,
            DefaultTradingInstrumentSettings defaultTradingInstrumentSettings)
        {
            _productsRepository = productsRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _underlyingsCache = underlyingsCache;
            _defaultTradingInstrumentSettings = defaultTradingInstrumentSettings;
        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetAsync(string tradingConditionId)
        {
            var activeAssetTypesIds = string.IsNullOrEmpty(tradingConditionId) 
                ? await _clientProfileSettingsRepository.GetActiveAssetTypeIdsAsync()
                : await _clientProfileSettingsRepository.GetActiveAssetTypeIdsAsync(tradingConditionId);
            
            var products = await _productsRepository.GetByAssetTypeIdsAsync(activeAssetTypesIds);
            
            return await GetTradingInstruments(products, tradingConditionId).ToListAsync();
        }

        public async Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null,
            int? skip = null, int? take = null)
        {

            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            var activeAssetTypesIds = string.IsNullOrEmpty(tradingConditionId) 
                ? await _clientProfileSettingsRepository.GetActiveAssetTypeIdsAsync()
                : await _clientProfileSettingsRepository.GetActiveAssetTypeIdsAsync(tradingConditionId);

            var products =
                await _productsRepository.GetPagedByAssetTypeIdsAsync(activeAssetTypesIds, skip.Value, take.Value);

            var tradingInstruments =
                await GetTradingInstruments(products.Contents, tradingConditionId).ToListAsync();
            
            return new PaginatedResponse<ITradingInstrument>(tradingInstruments, products.Start, products.Size, products.TotalSize);
        }

        public async Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId)
        {
            var productResult = await _productsRepository.GetByIdAsync(assetPairId);

            if (productResult.IsFailed)
                return null;

            var tradingInstruments = 
                GetTradingInstruments(new List<Product> {productResult.Value}, tradingConditionId);
            
            return await tradingInstruments.FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetUnavailableProductsAsync(List<string> productIds, string tradingConditionId)
        {
            productIds ??= new List<string>();

            var availableInstruments = await GetAsync(tradingConditionId);
            var availableProductIds = availableInstruments.Select(x => x.Instrument);

            return productIds.Except(availableProductIds).ToList();
        }

        private async IAsyncEnumerable<ITradingInstrument> GetTradingInstruments(
            IReadOnlyList<Product> products,
            string tradingConditionId = null)
        {
            var clientProfiles = tradingConditionId == null
                ? await _clientProfilesRepository.GetAllAsync()
                : new[] {await _clientProfilesRepository.GetByIdAsync(tradingConditionId)};

            var assetTypes = products.Select(p => p.AssetType).Distinct().ToList();
            
            foreach (var clientProfile in clientProfiles)
            {
                var availableClientProfileSettingsList = 
                    await _clientProfileSettingsRepository.GetAllAsync(clientProfile.Id, assetTypes, true);
                
                foreach (var product in products)
                {
                    var clientProfileSettings = availableClientProfileSettingsList.SingleOrDefault(x =>
                        x.ClientProfileId == clientProfile.Id && x.AssetTypeId == product.AssetType);
                    
                    if (clientProfileSettings == null)
                        continue;

                    var underlying = _underlyingsCache.GetByMdsCode(product.UnderlyingMdsCode);

                    yield return TradingInstrument.CreateFromProduct(
                        product,
                        clientProfileSettings.ClientProfileId,
                        clientProfileSettings.Margin,
                        underlying?.Spread ?? _defaultTradingInstrumentSettings.Spread);
                }
            }
        }
    }
}