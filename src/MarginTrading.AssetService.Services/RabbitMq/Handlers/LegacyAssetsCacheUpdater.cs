﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Services.Extensions;
using Microsoft.Extensions.Logging;
using Asset = MarginTrading.AssetService.Contracts.LegacyAsset.Asset;
using LegacyAssetExtensions = MarginTrading.AssetService.Services.Extensions.LegacyAssetExtensions;
using TickFormula = MarginTrading.AssetService.Core.Domain.TickFormula;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class LegacyAssetsCacheUpdater : ILegacyAssetsCacheUpdater
    {
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly Lykke.RabbitMqBroker.Publisher.IMessageProducer<AssetUpsertedEvent> _assetUpsertedPublisher;
        private readonly ILogger<LegacyAssetsCacheUpdater> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly IList<string> _assetTypesWithZeroInterestRate;

        public LegacyAssetsCacheUpdater(
            ILegacyAssetsService legacyAssetsService,
            ILegacyAssetsCache legacyAssetsCache, Lykke.RabbitMqBroker.Publisher.IMessageProducer<AssetUpsertedEvent> assetUpsertedPublisher,
            IList<string> assetTypesWithZeroInterestRate,
            ILogger<LegacyAssetsCacheUpdater> logger)
        {
            _legacyAssetsService = legacyAssetsService;
            _legacyAssetsCache = legacyAssetsCache;
            _assetUpsertedPublisher = assetUpsertedPublisher;
            _assetTypesWithZeroInterestRate = assetTypesWithZeroInterestRate;
            _logger = logger;
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
                    _logger.LogWarning("We received ProductChanged with productId: {ProductId} but cannot find it in DB to update LegacyAssetCache", product.ProductId);
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
            Func<Asset, bool> filter = x => x.Underlying.MarketDetails.MarketId == marketSettings.Id;
            await Handle(filter, timestamp);
        }

        public async Task HandleTickFormulaUpdated(TickFormula tickFormula, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.TickFormulaName == tickFormula.Id;
            await Handle(tickFormula, filter, LegacyAssetExtensions.SetAssetFieldsFromTickFormula, timestamp);
        }

        public async Task HandleProductCategoryUpdated(ProductCategory productCategory, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.CategoryRaw == productCategory.Id;
            await Handle(productCategory, filter, LegacyAssetExtensions.SetAssetFieldsFromCategory, timestamp);
        }

        public async Task HandleClientProfileSettingsUpdated(ClientProfileSettings clientProfileSettings, DateTime timestamp)
        {
            await Handle(x => x.Underlying.AssetType == clientProfileSettings.AssetTypeId, timestamp);
        }

        public async Task HandleCurrencyUpdated(string oldInterestRateMdsCode, Currency currency, DateTime timestamp)
        {
            Func<Asset, bool> tradingCurrencyFilter = x => x.Underlying.InterestRates.Any(c => c.Currency == currency.Id);
            await Handle(currency,
                tradingCurrencyFilter,
                (a, c) => a.SetAssetFieldsFromTradingCurrency(c, _assetTypesWithZeroInterestRate),
                timestamp);

            Func<Asset, bool> baseCurrencyFilter = x => x.Underlying.VariableInterestRate1 == oldInterestRateMdsCode;
            await Handle(currency, 
                baseCurrencyFilter, 
                (a, c) => a.SetAssetFieldsFromBaseCurrency(c, _assetTypesWithZeroInterestRate), 
                timestamp);
        }

        public async Task HandleUnderlyingUpdated(string oldMdsCode, UnderlyingsCacheModel underlying, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.Underlying.MdsCode == oldMdsCode;
            await Handle(underlying, filter, LegacyAssetExtensions.SetAssetFieldsFromUnderlying, timestamp);
        }

        public Task HandleAssetTypeUpdated(AssetType assetType, DateTime timestamp)
        {
            Func<Asset, bool> filter = x => x.Underlying.AssetType == assetType.Id;
            return Handle(filter, timestamp);
        }

        public Task UpdateAll(DateTime timestamp)
        {
            return Handle(x => true, timestamp);
        }

        public async Task HandleClientProfileChanged(DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
                return;

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

        private async Task Handle(Func<Asset, bool> getAffectedFilter,  DateTime timestamp)
        {
            if (timestamp < _legacyAssetsCache.CacheInitTimestamp)
            {
                _logger.LogInformation(
                    "Legacy asset cache update is discarded, source event is out of date. Details: {Details}", new
                    {
                        EventTimestamp = timestamp,
                        CacheInitiallizationTimestamp = _legacyAssetsCache.CacheInitTimestamp
                    }.ToJson());

                return;
            }

            await _semaphore.WaitAsync();
            try
            {
                var affectedAssets = _legacyAssetsCache.GetByFilter(getAffectedFilter);
                var updatedAssets = await _legacyAssetsService.GetLegacyAssets(affectedAssets.Select(x => (string)x.AssetId));

                if (updatedAssets.Count > 0)
                {
                    _legacyAssetsCache.AddOrUpdateMultiple(updatedAssets);
                    await PublishAssetUpsertedEvents(updatedAssets);

                    _logger.LogInformation("Updated {Count} assets in legacy cache", updatedAssets.Count);
                }
            }
            finally
            {
                _semaphore.Release();
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
            _logger.LogInformation("Published asset upserted event");
        }
    }
}