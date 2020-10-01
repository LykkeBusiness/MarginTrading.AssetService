using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Cronut.Dto.MessageBus;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;
using Asset = Cronut.Dto.Assets.Asset;

namespace MarginTrading.AssetService.Services.Handlers
{
    public class ReferentialDataChangedHandler
    {
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly IMessageProducer<AssetUpsertedEvent> _assetUpsertedPublisher;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ReferentialDataChangedHandler(
            ILegacyAssetsService legacyAssetsService,
            ILegacyAssetsCache legacyAssetsCache,
            IMessageProducer<AssetUpsertedEvent> assetUpsertedPublisher)
        {
            _legacyAssetsService = legacyAssetsService;
            _legacyAssetsCache = legacyAssetsCache;
            _assetUpsertedPublisher = assetUpsertedPublisher;
        }

        public async Task HandleProductUpserted(Product product)
        {
            var assets = await _legacyAssetsService.GetLegacyAssets(new List<string> { product.ProductId });
            string oldMdsCodeIfUpdated = null;

            try
            {
                await _semaphore.WaitAsync();
                var current = _legacyAssetsCache.GetById(product.ProductId);
                if (current != null && current.UnderlyingMdsCode != product.UnderlyingMdsCode)
                    oldMdsCodeIfUpdated = current.UnderlyingMdsCode;

                _legacyAssetsCache.AddOrUpdateMultiple(assets);
            }
            finally
            {
                _semaphore.Release();
            }

            await PublishAssetUpsertedEvents(assets, oldMdsCodeIfUpdated);
        }

        private async Task PublishAssetUpsertedEvents(List<Asset> assets, string oldMdsCodeIfUpdated = null)
        {
            foreach (var asset in assets)
            {
                await _assetUpsertedPublisher.ProduceAsync(new AssetUpsertedEvent
                {
                    Asset = asset,
                    EventMetadata = new EventMetadata(
                        eventId: Guid.NewGuid().ToString(),
                        eventCreationDate: DateTime.UtcNow),
                    PropertiesPriorValueIfUpdated = new AssetUpdatedProperties
                    {
                        UnderlyingMdsCode = oldMdsCodeIfUpdated
                    }
                });
            }
        }

        public async Task HandleMarketSettingsUpdated(MarketSettings marketSettings)
        {
            Func<Asset, bool> filter = x => x.Underlying.MarketName == marketSettings.Id;
            await Handle(marketSettings, filter, CronutAssetExtensions.SetAssetFieldsFromMarketSettings);
        }

        public async Task HandleTickFormulaUpdated(TickFormula tickFormula)
        {
            Func<Asset, bool> filter = x => x.TickFormulaName == tickFormula.Id;
            await Handle(tickFormula, filter, CronutAssetExtensions.SetAssetFieldsFromTickFormula);
        }

        public async Task HandleProductCategoryUpdated(ProductCategory productCategory)
        {
            Func<Asset, bool> filter = x => x.CategoryRaw == productCategory.Id;
            await Handle(productCategory, filter, CronutAssetExtensions.SetAssetFieldsFromCategory);
        }

        public async Task HandleClientProfileSettingsUpdated(ClientProfileSettings clientProfileSettings)
        {
            Func<Asset, bool> filter = x => x.Underlying.ExecutionFeeParameter.AssetType == clientProfileSettings.AssetTypeId;
            await Handle(clientProfileSettings, filter, CronutAssetExtensions.SetAssetFieldsFromClientProfileSettings);
        }

        public async Task HandleCurrencyUpdated(string oldInterestRateMdsCode, Currency currency)
        {
            Func<Asset, bool> tradingCurrencyFilter = x => x.Underlying.InterestRates.Any( c=> c.Currency == currency.Id);
            await Handle(currency, tradingCurrencyFilter, CronutAssetExtensions.SetAssetFieldsFromTradingCurrency);

            Func<Asset, bool> baseCurrencyFilter = x => x.Underlying.VariableInterestRate1 == oldInterestRateMdsCode;
            await Handle(currency, baseCurrencyFilter, CronutAssetExtensions.SetAssetFieldsFromBaseCurrency);
        }

        public async Task HandleUnderlyingUpdated(string oldMdsCode, UnderlyingsCacheModel underlying)
        {
            Func<Asset, bool> filter = x => x.Underlying.MdsCode == oldMdsCode;
            await Handle(underlying, filter, CronutAssetExtensions.SetAssetFieldsFromUnderlying);

            //TODO:BaseCurrency
        }

        private async Task Handle<T>(T changedEntity, Func<Asset, bool> getAffectedFilter, Action<Asset, T> dataModifier)
        {
            await _semaphore.WaitAsync();
            try
            {
                var affectedAssets = _legacyAssetsCache.GetByFilter(getAffectedFilter);

                foreach (var affectedAsset in affectedAssets)
                {
                    dataModifier(affectedAsset, changedEntity);
                }

                //TODO:Raise events

            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}