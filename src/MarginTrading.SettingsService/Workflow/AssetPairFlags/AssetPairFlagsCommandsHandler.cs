using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        
        public AssetPairFlagsCommandsHandler(
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService)
        {
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
        }
        
        /// <summary>
        /// Suspend asset pair
        /// </summary>
        [UsedImplicitly]
        private async Task<CommandHandlingResult> Handle(SuspendAssetPairCommand command,
            IEventPublisher publisher)
        {
            //idempotency handling not required

            var assetPair = await _assetPairsRepository.ChangeSuspendFlag(command.AssetPairId, true);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });
            
            return CommandHandlingResult.Ok();
        }

        /// <summary>
        /// Unsuspend asset pair
        /// </summary>
        [UsedImplicitly]
        private async Task<CommandHandlingResult> Handle(UnsuspendAssetPairCommand command,
            IEventPublisher publisher)
        {
            //idempotency handling not required
            
            var assetPair = await _assetPairsRepository.ChangeSuspendFlag(command.AssetPairId, false);
            
            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });
            
            return CommandHandlingResult.Ok();
        }
    }
}