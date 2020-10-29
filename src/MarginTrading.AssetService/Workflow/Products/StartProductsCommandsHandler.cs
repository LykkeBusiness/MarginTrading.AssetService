using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;


namespace MarginTrading.AssetService.Workflow.Products
{
    public class StartProductsCommandsHandler
    {
        private string username = "system";
        
        private readonly IProductsRepository _productsRepository;
        private readonly IAuditService _auditService;
        private readonly IConvertService _convertService;
        private readonly ILog _log;

        public StartProductsCommandsHandler(IProductsRepository productsRepository,
            IAuditService auditService,
            IConvertService convertService,
            ILog log
            )
        {
            _productsRepository = productsRepository;
            _auditService = auditService;
            _convertService = convertService;
            _log = log;
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
                    await _auditService.TryAudit(command.OperationId, username, product.ProductId, AuditDataType.Product,
                        product.ToJson(), existing.Value.ToJson());
                    
                    publisher.PublishEvent(new ProductChangedEvent()
                    {
                        Username = username,
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
                    _log.WriteWarning(nameof(StartProductsCommandsHandler), nameof(Handle), 
                        $"Attempt to start product with id {command.ProductId} failed");
                    
                    throw new Exception($"Could not start product with id {command.ProductId}");
                }
            }
            else
            {
                _log.WriteWarning(nameof(StartProductsCommandsHandler), nameof(Handle), 
                   $"Could not find product with id {command.ProductId} to start it");
                
            }
        }
    }
}