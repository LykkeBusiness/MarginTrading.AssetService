using System.Threading.Tasks;
using Lykke.Snow.Audit;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    /// <summary>
    /// It is implemented as a separate service to avoid circular dependency during IOC registration of ICqrsEngine
    /// </summary>
    public class ProductsDiscontinueService : IProductsDiscontinueService
    {
        private readonly IProductsRepository _repository;
        private readonly IAuditService _auditService;

        public ProductsDiscontinueService(IProductsRepository repository,
            IAuditService auditService)
        {
            _repository = repository;
            _auditService = auditService;
        }

        public async Task<(Product OldValue, Product NewValue, bool IsSuccess)> ChangeSuspendStatusAsync(
            string productId,
            bool isSuspended,
            string username, string correlationId)
        {
            var existing = await _repository.GetByIdAsync(productId);
            if (existing.IsFailed) return (null, null, false);

            var result = await _repository.ChangeSuspendFlagAsync(productId, isSuspended);
            if (result.IsSuccess)
            {
                var product = result.Value;

                await _auditService.CreateAuditRecord(AuditEventType.Edition, username, product, existing.Value);

                return (existing.Value, result.Value, true);
            }

            return (null, null, false);
        }
    }
}