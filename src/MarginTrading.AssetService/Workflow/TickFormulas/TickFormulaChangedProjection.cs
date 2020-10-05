using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Handlers;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Workflow.TickFormulas
{
    public class TickFormulaChangedProjection
    {
        private readonly ILegacyAssetsCacheUpdater _legacyAssetsCacheUpdater;
        private readonly IConvertService _convertService;

        public TickFormulaChangedProjection(ILegacyAssetsCacheUpdater legacyAssetsCacheUpdater, IConvertService convertService)
        {
            _legacyAssetsCacheUpdater = legacyAssetsCacheUpdater;
            _convertService = convertService;
        }

        [UsedImplicitly]
        public async Task Handle(TickFormulaChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    break;
                case ChangeType.Edition:
                    await _legacyAssetsCacheUpdater.HandleTickFormulaUpdated(
                        _convertService.Convert<TickFormulaContract, TickFormula>(e.NewValue), e.Timestamp);
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}