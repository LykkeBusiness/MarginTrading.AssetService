using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class UnderlyingChangedHandler : IMessageHandler<UnderlyingChangedEvent>
    {
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IProductsService _productsService;
        private readonly IConvertService _convertService;

        public UnderlyingChangedHandler(
            IUnderlyingsCache underlyingsCache,
            ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater,
            IProductsService productsService,
            IConvertService convertService)
        {
            _underlyingsCache = underlyingsCache;
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _productsService = productsService;
            _convertService = convertService;
        }

        public async Task Handle(UnderlyingChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    _underlyingsCache.AddOrUpdateByMdsCode(
                        _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue));
                    break;
                case ChangeType.Edition:

                    var model = _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue);

                    await UpdateUnderlyingsCache(e, model);
                    await HandleMdsCodeChanged(e);

                    await _legacyAssetsCacheUpdater.HandleUnderlyingUpdated(e.OldValue.MdsCode, model, e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    _underlyingsCache.Remove(
                        _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.OldValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Task UpdateUnderlyingsCache(UnderlyingChangedEvent e, UnderlyingsCacheModel model)
        {
            if (e.OldValue.MdsCode != e.NewValue.MdsCode)
            {
                _underlyingsCache.AddOrUpdateByChangedMdsCode(e.OldValue.MdsCode, model);
            }
            else
            {
                _underlyingsCache.AddOrUpdateByMdsCode(model);
            }
            
            return Task.CompletedTask;
        }

        private async Task HandleMdsCodeChanged(UnderlyingChangedEvent e)
        {
            if (e.OldValue.MdsCode != e.NewValue.MdsCode)
            {
                try
                {
                    await _productsService.ChangeUnderlyingMdsCodeAsync(e.OldValue.MdsCode,
                        e.NewValue.MdsCode,
                        e.Username);
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        $"Cannot update products with underlying mds code {e.OldValue.MdsCode}: {exception.Message}",
                        exception);
                }
            }
        }
    }
}