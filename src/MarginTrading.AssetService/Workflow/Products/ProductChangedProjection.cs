using System;
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

        public ProductChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, IConvertService convertService)
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
                    if (!e.NewValue.IsStarted) return;
                    await _legacyAssetsCacheUpdater.HandleProductUpserted(_convertService.Convert<ProductContract, Product>(e.NewValue), e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    if (!e.OldValue.IsStarted) return;
                    await _legacyAssetsCacheUpdater.HandleProductRemoved(e.OldValue.ProductId, e.Timestamp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}