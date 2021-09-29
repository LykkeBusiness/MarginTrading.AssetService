// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private string username = "system";

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(15);
        private readonly IProductsDiscontinueService _productsDiscontinueService;
        private readonly IProductsRepository _productsRepository;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        private readonly IConvertService _convertService;
        private readonly IChaosKitty _chaosKitty;
        private readonly ILog _log;

        public AssetPairFlagsCommandsHandler(
            IProductsDiscontinueService productsDiscontinueService,
            IProductsRepository productsRepository,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            IConvertService convertService,
            IChaosKitty chaosKitty,
            ILog log)
        {
            _productsDiscontinueService = productsDiscontinueService;
            _productsRepository = productsRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
            _convertService = convertService;
            _chaosKitty = chaosKitty;
            _log = log;
        }

        /// <summary>
        /// Suspend asset pair
        /// </summary>
        [Obsolete("Migrate to ChangeProductSuspendedStatusCommand handler. Left for backwards compatibility during migration")]
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
        [Obsolete("Migrate to ChangeProductSuspendedStatusCommand handler. Left for backwards compatibility during migration")]
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
        
        [UsedImplicitly]
        private async Task<CommandHandlingResult> Handle(ChangeProductSuspendedStatusCommand command,
            IEventPublisher publisher)
        {
            var getProductResult = await _productsRepository.GetByIdAsync(command.ProductId);
            if (getProductResult.IsFailed)
            {
                _log.WriteWarning(nameof(AssetPairFlagsCommandsHandler), nameof(Handle), 
                    $"Product {command.ProductId} not found");
                return CommandHandlingResult.Fail(_delay);
            }

            var product = getProductResult.Value;

            if (product.IsSuspended == command.IsSuspended)
            {
                _log.WriteWarning(nameof(AssetPairFlagsCommandsHandler), nameof(Handle), 
                    $"IsSuspended status on both product {command.ProductId} and event are same: on product {product.IsSuspended}, on command {command.IsSuspended}. Update is skipped");
                return CommandHandlingResult.Ok();
            }

            var updateResult = await _productsDiscontinueService.ChangeSuspendStatusAsync(command.ProductId,
                command.IsSuspended,
                username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _log.WriteInfo(nameof(AssetPairFlagsCommandsHandler), nameof(Handle), 
                $"IsSuspended status on product {command.ProductId} is updated successfully. ChangeAssetPairSuspendedStatusCommand Timestamp: {command.Timestamp}, new status: {command.IsSuspended}");
            
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