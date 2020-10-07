// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace MarginTrading.AssetService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(15);
        private readonly IAssetPairService _assetPairService;
        private readonly IConvertService _convertService;
        private readonly IChaosKitty _chaosKitty;
        
        public AssetPairFlagsCommandsHandler(
            IAssetPairService assetPairService,
            IConvertService convertService,
            IChaosKitty chaosKitty)
        {
            _assetPairService = assetPairService;
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
            var assetPair = await _assetPairService.ChangeSuspendStatusAsync(command.AssetPairId, true);

            if (assetPair == null)
                return CommandHandlingResult.Fail(_delay);

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
            var assetPair = await _assetPairService.ChangeSuspendStatusAsync(command.AssetPairId, false);

            _chaosKitty.Meow(command.OperationId);

            if(assetPair == null)
                return CommandHandlingResult.Fail(_delay);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });

            return CommandHandlingResult.Ok();
        }
    }
}