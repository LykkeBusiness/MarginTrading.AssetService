using System.Threading.Tasks;
using Common;
using Lykke.Snow.Mdm.Contracts.Models.Events;
using MarginTrading.AssetService.Core.Handlers;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services.RabbitMq.Handlers
{
    public class BrokerSettingsChangedHandler
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly ILogger<BrokerSettingsChangedHandler> _logger;

        public BrokerSettingsChangedHandler(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, ILogger<BrokerSettingsChangedHandler> logger)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _logger = logger;
        }

        public async Task Handle(BrokerSettingsChangedEvent e)
        {
            if (NeedToInvalidateCache(e))
            {
                _logger.LogInformation("Invalidating cache for all legacy assets, context: {Context}", e.ToJson());
                await _legacyAssetsCacheUpdater.UpdateAll(e.Timestamp);

            }
            else
            {
                _logger.LogInformation("Cache invalidation not required, context: {Context}", e.ToJson());
            }
        }

        private static bool NeedToInvalidateCache(BrokerSettingsChangedEvent e)
        {
            if (e.ChangeType != ChangeType.Edition)
            {
                return false;
            }

            //invalidating cache only if fields related to assets changed
            //see CronutAssetExtensions.SetDividendFactorFields for details
            return !e.NewValue.Dividends871MPercent.Equals(e.OldValue.Dividends871MPercent)
                   || !e.NewValue.DividendsLongPercent.Equals(e.OldValue.DividendsLongPercent)
                   || !e.NewValue.DividendsShortPercent.Equals(e.OldValue.DividendsShortPercent);
        }
    }
}