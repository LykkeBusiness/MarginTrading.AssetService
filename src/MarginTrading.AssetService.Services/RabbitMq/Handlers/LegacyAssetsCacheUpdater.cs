using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Cronut.Dto.MessageBus;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;
using Asset = Cronut.Dto.Assets.Asset;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class LegacyAssetsCacheUpdater : ILegacyAssetsCacheUpdater
    {
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly IMessageProducer<AssetUpsertedEvent> _assetUpsertedPublisher;
        private readonly ILog _log;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public LegacyAssetsCacheUpdater(
            ILegacyAssetsService legacyAssetsService,
            ILegacyAssetsCache legacyAssetsCache,
            IMessageProducer<AssetUpsertedEvent> assetUpsertedPublisher,
            ILog log)
        {
            _legacyAssetsService = legacyAssetsService;
            _legacyAssetsCache = legacyAssetsCache;
            _assetUpsertedPublisher = assetUpsertedPublisher;
            _log = log;
        }

        public async Task HandleProductRemoved(string productId, DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
                return;

            try
            {
                await _semaphore.WaitAsync();
                _legacyAssetsCache.Remove(productId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task HandleProductUpserted(Product product, DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
                return;
            
            if(!product.IsStarted)
                return;

            var assets = await _legacyAssetsService.GetLegacyAssets(new List<string> { product.ProductId });
            string oldMdsCodeIfUpdated = null;

            try
            {
                await _semaphore.WaitAsync();
                var current = _legacyAssetsCache.GetById(product.ProductId);
                if (current != null && current.UnderlyingMdsCode != product.UnderlyingMdsCode)
                    oldMdsCodeIfUpdated = current.UnderlyingMdsCode;

                _legacyAssetsCache.AddOrUpdateMultiple(assets);

                //We must have one asset for productId
                var asset = assets.FirstOrDefault();

                if (asset == null)
                {
                    _log.WriteWarning(nameof(LegacyAssetsCacheUpdater), nameof(HandleProductUpserted),
                        $"We received ProductChanged with productId: {product.ProductId} but cannot find it in DB to update LegacyAssetCache");
                    return;
                }

                await PublishAssetUpsertedEvent(asset, oldMdsCodeIfUpdated);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task HandleMarketSettingsUpdated(MarketSettings marketSettings, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.Underlying.MarketName == marketSettings.Id;
            await Handle(marketSettings, filter, CronutAssetExtensions.SetAssetFieldsFromMarketSettings, timestamp);
        }

        public async Task HandleTickFormulaUpdated(TickFormula tickFormula, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.TickFormulaName == tickFormula.Id;
            await Handle(tickFormula, filter, CronutAssetExtensions.SetAssetFieldsFromTickFormula, timestamp);
        }

        public async Task HandleProductCategoryUpdated(ProductCategory productCategory, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.CategoryRaw == productCategory.Id;
            await Handle(productCategory, filter, CronutAssetExtensions.SetAssetFieldsFromCategory, timestamp);
        }

        public async Task HandleClientProfileSettingsUpdated(ClientProfileSettings clientProfileSettings, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.Underlying.ExecutionFeeParameter.AssetType == clientProfileSettings.AssetTypeId;
            await Handle(clientProfileSettings, filter, CronutAssetExtensions.SetAssetFieldsFromClientProfileSettings, timestamp);
        }

        public async Task HandleCurrencyUpdated(string oldInterestRateMdsCode, Currency currency, DateTime timestamp)
        {
            Func<Asset, bool> tradingCurrencyFilter = x => x.Underlying.InterestRates.Any(c => c.Currency == currency.Id);
            await Handle(currency, tradingCurrencyFilter, CronutAssetExtensions.SetAssetFieldsFromTradingCurrency, timestamp);

            Func<Asset, bool> baseCurrencyFilter = x => x.Underlying.VariableInterestRate1 == oldInterestRateMdsCode;
            await Handle(currency, baseCurrencyFilter, CronutAssetExtensions.SetAssetFieldsFromBaseCurrency, timestamp);
        }

        public async Task HandleUnderlyingUpdated(string oldMdsCode, UnderlyingsCacheModel underlying, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.Underlying.MdsCode == oldMdsCode;
            await Handle(underlying, filter, CronutAssetExtensions.SetAssetFieldsFromUnderlying, timestamp);
        }

        public async Task HandleClientProfileUpserted(ClientProfile old, ClientProfile updated, DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
                return;

            if (updated.IsDefault && (old == null || !old.IsDefault))
            {
                //If default client profile is changed all assets will be affected
                var reinitializedAssets = await _legacyAssetsService.GetLegacyAssets();

                await _semaphore.WaitAsync();
                try
                {
                    _legacyAssetsCache.AddOrUpdateMultiple(reinitializedAssets);
                    await PublishAssetUpsertedEvents(reinitializedAssets);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private async Task Handle<T>(T changedEntity, Func<Asset, bool> getAffectedFilter, Action<Asset, T> dataModifier, DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
                return;

            await _semaphore.WaitAsync();
            try
            {
                var affectedAssets = _legacyAssetsCache.GetByFilter(getAffectedFilter);

                foreach (var affectedAsset in affectedAssets)
                {
                    dataModifier(affectedAsset, changedEntity);
                }

                _legacyAssetsCache.AddOrUpdateMultiple(affectedAssets);

                await PublishAssetUpsertedEvents(affectedAssets);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task PublishAssetUpsertedEvents(List<Asset> assets)
        {
            foreach (var asset in assets)
            {
                await PublishAssetUpsertedEvent(asset);
            }
        }

        private async Task PublishAssetUpsertedEvent(Asset asset, string oldMdsCodeIfUpdated = null)
        {
            var evt = new AssetUpsertedEvent
            {
                Asset = asset,
                EventMetadata = new EventMetadata(
                    eventId: Guid.NewGuid().ToString(),
                    eventCreationDate: DateTime.UtcNow),
                PropertiesPriorValueIfUpdated = new AssetUpdatedProperties
                {
                    UnderlyingMdsCode = oldMdsCodeIfUpdated
                }
            };
            await _assetUpsertedPublisher.ProduceAsync(evt);
            _log.WriteInfo(nameof(LegacyAssetsCacheUpdater), evt, "Published asset upserted event");
        }
    }
}