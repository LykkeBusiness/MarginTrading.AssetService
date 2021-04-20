using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Core.Handlers;

namespace MarginTrading.AssetService.Workflow.ClientProfiles
{
    public class ClientProfileChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;

        public ClientProfileChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
        }

        [UsedImplicitly]
        public Task Handle(ClientProfileChangedEvent e)
            => _legacyAssetsCacheUpdater.HandleClientProfileChanged(e.Timestamp);
    }
}