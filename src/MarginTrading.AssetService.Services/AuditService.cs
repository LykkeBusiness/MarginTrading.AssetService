using System;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common;
using Lykke.Snow.Common.Correlation;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly CorrelationContextAccessor _correlationContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IAuditRepository auditRepository, CorrelationContextAccessor correlationContextAccessor, ILogger<AuditService> logger)
        {
            _auditRepository = auditRepository;
            _correlationContextAccessor = correlationContextAccessor;
            _logger = logger;
        }

        public Task<PaginatedResponse<IAuditModel<AuditDataType>>> GetAll(AuditTrailFilter<AuditDataType> filter, int? skip, int? take)
        {
            (int s, int t) = PaginationUtils.ValidateSkipAndTake(skip, take);
            return _auditRepository.GetAll(filter, s, t);
        }

        public async Task CreateAuditRecord(AuditEventType eventType,
            string userName,
            IAuditableObject<AuditDataType> current,
            IAuditableObject<AuditDataType> original = null)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var correlationId = _correlationContextAccessor.GetOrGenerateCorrelationId();

            var model = current.GetAuditModel(original, eventType, () => (correlationId, userName));

            if (model == null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        "Audit model is null for eventType [{EventType}], object [{CurrentJson}, original [{OriginalJson}]",
                        eventType, current?.ToJson(), original?.ToJson());
                }
                else
                {
                    _logger.LogWarning("Audit model is null");   
                }
                return;
            }

            await _auditRepository.InsertAsync(model);
        }
    }
}