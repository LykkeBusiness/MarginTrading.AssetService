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
            if (NeedToInvalidateCache(e))
            {
                _log.Info("Invalidating cache for all legacy assets", context:e);
                await _legacyAssetsCacheUpdater.UpdateAll(e.Timestamp);

            }
            else
            {
                _log.Info("Cache invalidation not required", context: e);
            }
        }

        private static bool NeedToInvalidateCache(BrokerSettingsChangedEvent e)
        {
            //invalidating cache only if fields related to assets changed
            //see CronutAssetExtensions.SetDividendFactorFields for details
            return !e.NewValue.Dividends871MPercent.Equals(e.OldValue.Dividends871MPercent)
                   || !e.NewValue.DividendsLongPercent.Equals(e.OldValue.DividendsLongPercent)
                   || !e.NewValue.DividendsShortPercent.Equals(e.OldValue.DividendsShortPercent);
        }
    }
}