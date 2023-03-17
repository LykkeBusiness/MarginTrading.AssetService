using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.AssetTypes
{
    public class AssetTypeChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public AssetTypeChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, IConvertService convertService)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(AssetTypeChangedEvent e)
        {
            if (e.ChangeType != ChangeType.Edition)
                return;

            await _legacyAssetsCacheUpdater.HandleAssetTypeUpdated(
                _convertService.Convert<AssetTypeContract, AssetType>(e.NewValue), e.Timestamp);
        }
    }
}