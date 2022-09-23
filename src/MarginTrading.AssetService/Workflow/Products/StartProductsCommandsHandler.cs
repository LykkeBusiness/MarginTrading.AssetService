using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Snow.Audit;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Workflow.Products
{
    public class StartProductsCommandsHandler
    {
        private const string Username = "system";

        private readonly IProductsRepository _productsRepository;
        private readonly IAuditService _auditService;
        private readonly IConvertService _convertService;
        private readonly ILogger<StartProductsCommandsHandler> _logger;

        public StartProductsCommandsHandler(IProductsRepository productsRepository,
            IAuditService auditService,
            IConvertService convertService,
            ILogger<StartProductsCommandsHandler> logger)
        {
            _productsRepository = productsRepository;
            _auditService = auditService;
            _convertService = convertService;
            _logger = logger;
        }
        
        [UsedImplicitly]
        public async Task Handle(StartProductCommand command, IEventPublisher publisher)
        {
            var existing = await _productsRepository.GetByIdAsync(command.ProductId);

            if (existing.IsSuccess)
            {
                var product = existing.Value.ShallowCopy();
                product.IsStarted = true;

                var result = await _productsRepository.UpdateAsync(product);

                if (result.IsSuccess)
                {
                    await _auditService.CreateAuditRecord(AuditEventType.Edition, Username, product, existing.Value);
                    
                    publisher.PublishEvent(new ProductChangedEvent()
                    {
                        Username = Username,
                        ChangeType = ChangeType.Edition,
                        CorrelationId = command.OperationId,
                        EventId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow,
                        OldValue = _convertService.Convert<Product, ProductContract>(existing.Value),
                        NewValue = _convertService.Convert<Product, ProductContract>(product),
                    });
                }
                else
                {
                    _logger.LogWarning("Attempt to start product with id {ProductId} failed", command.ProductId);
                    throw new Exception($"Could not start product with id {command.ProductId}");
                }
            }
            else
            {
                _logger.LogWarning("Could not find product with id {ProductId} to start it", command.ProductId);
            }
        }
    }
}