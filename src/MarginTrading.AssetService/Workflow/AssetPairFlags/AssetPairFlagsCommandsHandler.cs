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
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Workflow.AssetPairFlags
{
    public class AssetPairFlagsCommandsHandler
    {
        private const string Username = "system";

        private readonly TimeSpan _delay = TimeSpan.FromSeconds(15);
        private readonly IProductsDiscontinueService _productsDiscontinueService;
        private readonly IProductsRepository _productsRepository;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        private readonly IConvertService _convertService;
        private readonly IChaosKitty _chaosKitty;
        private readonly ILogger<AssetPairFlagsCommandsHandler> _logger;

        public AssetPairFlagsCommandsHandler(
            IProductsDiscontinueService productsDiscontinueService,
            IProductsRepository productsRepository,
            DefaultLegalEntitySettings defaultLegalEntitySettings,
            IConvertService convertService,
            IChaosKitty chaosKitty,
            ILogger<AssetPairFlagsCommandsHandler> logger)
        {
            _productsDiscontinueService = productsDiscontinueService;
            _productsRepository = productsRepository;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
            _convertService = convertService;
            _chaosKitty = chaosKitty;
            _logger = logger;
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
                Username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _chaosKitty.Meow(command.OperationId);

            var assetPair =
                AssetPair.CreateFromProduct(updateResult.NewValue, _defaultLegalEntitySettings.DefaultLegalEntity);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair)
            });

            publisher.PublishEvent(CreateProductChangedEvent(updateResult.OldValue,
                updateResult.NewValue,
                Username,
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
                Username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _chaosKitty.Meow(command.OperationId);

            var assetPair =
                AssetPair.CreateFromProduct(updateResult.NewValue, _defaultLegalEntitySettings.DefaultLegalEntity);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair)
            });

            publisher.PublishEvent(CreateProductChangedEvent(updateResult.OldValue,
                updateResult.NewValue,
                Username,
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
                _logger.LogWarning("Product {ProductId} not found", command.ProductId);
                return CommandHandlingResult.Fail(_delay);
            }

            var product = getProductResult.Value;

            if (product.IsSuspended == command.IsSuspended)
            {
                _logger.LogWarning(
                    "IsSuspended status on both product {ProductId} and event are same: on product {ProductIsSuspended}, on command {CommandIsSuspended}. Update is skipped",
                    command.ProductId, product.IsSuspended, command.IsSuspended);
                return CommandHandlingResult.Ok();
            }

            var updateResult = await _productsDiscontinueService.ChangeSuspendStatusAsync(command.ProductId,
                command.IsSuspended,
                Username,
                command.OperationId);

            if (!updateResult.IsSuccess)
                return CommandHandlingResult.Fail(_delay);

            _logger.LogInformation(
                "IsSuspended status on product {ProductId} is updated successfully. ChangeAssetPairSuspendedStatusCommand Timestamp: {Timestamp}, new status: {IsSuspended}",
                command.ProductId, command.Timestamp, command.IsSuspended);
            
            _chaosKitty.Meow(command.OperationId);

            var assetPair =
                AssetPair.CreateFromProduct(updateResult.NewValue, _defaultLegalEntitySettings.DefaultLegalEntity);

            publisher.PublishEvent(new AssetPairChangedEvent
            {
                OperationId = command.OperationId,
                AssetPair = _convertService.Convert<IAssetPair, AssetPairContract>(assetPair)
            });

            publisher.PublishEvent(CreateProductChangedEvent(updateResult.OldValue,
                updateResult.NewValue,
                Username,
                command.OperationId));

            return CommandHandlingResult.Ok();
        }

        private ProductChangedEvent CreateProductChangedEvent(Product oldValue, Product newValue, string username,
            string correlationId)
        {
            return new ProductChangedEvent
            {
                Username = username,
                ChangeType = ChangeType.Edition,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<Product, ProductContract>(oldValue),
                NewValue = _convertService.Convert<Product, ProductContract>(newValue)
            };
        }
    }
}