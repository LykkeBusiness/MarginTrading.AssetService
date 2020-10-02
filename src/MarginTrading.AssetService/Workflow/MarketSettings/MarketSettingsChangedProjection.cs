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
        private readonly IReferentialDataChangedHandler _referentialDataChangedHandler;
        private readonly IConvertService _convertService;

        public MarketSettingsChangedProjection(IReferentialDataChangedHandler referentialDataChangedHandler, IConvertService convertService)
        {
            _referentialDataChangedHandler = referentialDataChangedHandler;
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
                    await _referentialDataChangedHandler.HandleMarketSettingsUpdated(
                        _convertService.Convert<MarketSettingsContract, Core.Domain.MarketSettings>(e.NewMarketSettings));
                    break;
                case ChangeType.Deletion:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}