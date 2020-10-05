using System;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class UnderlyingChangedHandler
    {
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public UnderlyingChangedHandler(
            IUnderlyingsCache underlyingsCache,
            ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater,
            IConvertService convertService)
        {
            _underlyingsCache = underlyingsCache;
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        public Task Handle(UnderlyingChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    _underlyingsCache.AddOrUpdateByMdsCode(_convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue));
                    break;
                case ChangeType.Edition:

                    var model = _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(e.NewValue);
                    if (e.OldValue.MdsCode != e.NewValue.MdsCode)
                        _underlyingsCache.AddOrUpdateByChangedMdsCode(e.OldValue.MdsCode, model);
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

            return Task.CompletedTask;
        }
    }
}