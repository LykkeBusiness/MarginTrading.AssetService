// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace MarginTrading.SettingsService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        private readonly IChaosKitty _chaosKitty;
        
        public AssetPairFlagsCommandsHandler(
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService,
            IChaosKitty chaosKitty)
        {
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
            _chaosKitty = chaosKitty;
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
            
            _chaosKitty.Meow(command.OperationId);

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
            
            _chaosKitty.Meow(command.OperationId);
            
            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });
            
            return CommandHandlingResult.Ok();
        }
    }
}