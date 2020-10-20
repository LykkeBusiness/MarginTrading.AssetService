using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.MarketSettings
{
    public class MarketSettingsChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public MarketSettingsChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, IConvertService convertService)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(MarketSettingsChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    break;
                case ChangeType.Edition:
                    await _legacyAssetsCacheUpdater.HandleMarketSettingsUpdated(
                        _convertService.Convert<MarketSettingsContract, Core.Domain.MarketSettings>(e.NewMarketSettings), e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}