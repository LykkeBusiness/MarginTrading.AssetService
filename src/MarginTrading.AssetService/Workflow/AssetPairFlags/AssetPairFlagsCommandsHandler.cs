// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private string username = "system";

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(15);
        private readonly IProductsDiscontinueService _productsDiscontinueService;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        private readonly IConvertService _convertService;
        private readonly IChaosKitty _chaosKitty;

        public AssetPairFlagsCommandsHandler(
            IProductsDiscontinueService productsDiscontinueService,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            IConvertService convertService,
            IChaosKitty chaosKitty)
        {
            _productsDiscontinueService = productsDiscontinueService;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
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
            var updateResult = await _productsDiscontinueService.ChangeSuspendStatusAsync(command.AssetPairId,
                true,
                username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _chaosKitty.Meow(command.OperationId);

            var assetPair =
                AssetPair.CreateFromProduct(updateResult.NewValue, _defaultLegalEntitySettings.DefaultLegalEntity);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });

            publisher.PublishEvent(CreateProductChangedEvent(updateResult.OldValue,
                updateResult.NewValue,
                username,
                command.OperationId));

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
            var updateResult = await _productsDiscontinueService.ChangeSuspendStatusAsync(command.AssetPairId,
                false,
                username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _chaosKitty.Meow(command.OperationId);

            var assetPair =
                AssetPair.CreateFromProduct(updateResult.NewValue, _defaultLegalEntitySettings.DefaultLegalEntity);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair),
            });

            publisher.PublishEvent(CreateProductChangedEvent(updateResult.OldValue,
                updateResult.NewValue,
                username,
                command.OperationId));

            return CommandHandlingResult.Ok();
        }

        private ProductChangedEvent CreateProductChangedEvent(Product oldValue, Product newValue, string username,
            string correlationId)
        {
            return new ProductChangedEvent()
            {
                Username = username,
                ChangeType = ChangeType.Edition,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<Product, ProductContract>(oldValue),
                NewValue = _convertService.Convert<Product, ProductContract>(newValue),
            };
        }
    }
}