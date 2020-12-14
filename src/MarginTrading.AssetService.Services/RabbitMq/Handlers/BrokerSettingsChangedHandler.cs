using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Handlers;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class BrokerSettingsChangedHandler
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly ILog _log;

        public BrokerSettingsChangedHandler(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater,
            ILog log)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _log = log;
        }

        public async Task Handle(BrokerSettingsChangedEvent e)
        {
            _log.Info("Invalidating cache for all legacy assets");
            await _legacyAssetsCacheUpdater.UpdateAll(e.Timestamp);
        }
    }
}