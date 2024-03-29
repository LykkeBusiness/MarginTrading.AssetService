﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class ProductChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public ProductChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, 
            IConvertService convertService)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(ProductChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                case ChangeType.Edition:
                    if (!(e.NewValue is { IsStarted: true })) return;
                    var product = _convertService.Convert<ProductContract, Product>(e.NewValue);
                    await _legacyAssetsCacheUpdater.HandleProductUpserted(product, e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    if (!(e.OldValue is { IsStarted: true })) return;
                    await _legacyAssetsCacheUpdater.HandleProductRemoved(e.OldValue.ProductId, e.Timestamp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), $"Unexpected ChangeType: [{e.ChangeType}]");
            }
        }
    }
}