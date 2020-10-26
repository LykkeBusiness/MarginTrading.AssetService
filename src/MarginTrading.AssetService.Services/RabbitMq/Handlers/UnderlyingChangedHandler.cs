using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class UnderlyingChangedHandler
    {
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IProductsService _productsService;
        private readonly IConvertService _convertService;
        private readonly ILog _log;

        public UnderlyingChangedHandler(
            IUnderlyingsCache underlyingsCache,
            ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater,
            IProductsService productsService,
            IConvertService convertService,
            ILog log)
        {
            _underlyingsCache = underlyingsCache;
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _productsService = productsService;
            _convertService = convertService;
            _log = log;
        }

        public async Task Handle(UnderlyingChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    _underlyingsCache.AddOrUpdateByMdsCode(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue));
                    break;
                case ChangeType.Edition:

                    var model = _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue);
                    if (e.OldValue.MdsCode != e.NewValue.MdsCode)
                    {
                        _underlyingsCache.AddOrUpdateByChangedMdsCode(e.OldValue.MdsCode, model);
                        var productUpdateResult = await _productsService.ChangeUnderlyingMdsCodeAsync(e.OldValue.MdsCode, 
                            e.NewValue.MdsCode,
                            e.Username,
                            e.CorrelationId);
                        if (productUpdateResult.IsFailed)
                        {
                            if (productUpdateResult.Error == ProductsErrorCodes.DoesNotExist)
                            {
                                _log.WriteInfo(nameof(UnderlyingChangedHandler), nameof(Handle), 
                                    $"Cannot update a product with underlying mds code {e.OldValue.MdsCode}: product not found");
                            }
                            else
                            {
                                throw new Exception($"Cannot update a product with underlying mds code {e.OldValue.MdsCode}: {productUpdateResult.Error.ToString()}");
                            }
                        }
                    }
                    else
                        _underlyingsCache.AddOrUpdateByMdsCode(model);

                    _legacyAssetsCacheUpdater.HandleUnderlyingUpdated(e.OldValue.MdsCode, model, e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    _underlyingsCache.Remove(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.OldValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}