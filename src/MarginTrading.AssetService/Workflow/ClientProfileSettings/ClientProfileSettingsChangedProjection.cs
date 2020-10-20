using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.ClientProfileSettings
{
    public class ClientProfileSettingsChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public ClientProfileSettingsChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, IConvertService convertService)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(ClientProfileSettingsChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    break;
                case ChangeType.Edition:
                    await _legacyAssetsCacheUpdater.HandleClientProfileSettingsUpdated(
                        _convertService.Convert<ClientProfileSettingsContract, Core.Domain.ClientProfileSettings>(e.NewValue), e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}