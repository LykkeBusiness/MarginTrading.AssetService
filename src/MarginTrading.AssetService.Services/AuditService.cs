using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using JsonDiffPatchDotNet;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Extensions;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILog _log;

        public AuditService(IAuditRepository auditRepository, ILog log)
        {
            _auditRepository = auditRepository;
            _log = log;
        }

        public Task<IReadOnlyList<IAuditModel>> GetAll(AuditLogsFilterDto filter)
            => _auditRepository.GetAll(filter);

        public async Task<bool> TryAudit(
            string correlationId,
            string userName,
            string referenceId,
            AuditDataType type,
            string newStateJson = null,
            string oldStateJson = null)
        {
            if (string.IsNullOrEmpty(newStateJson) && string.IsNullOrEmpty(oldStateJson))
            {
                _log?.WriteWarningAsync(nameof(AuditService), nameof(TryAudit),
                    $"Unable to generate audit event based on both {nameof(newStateJson)} and {nameof(oldStateJson)} state as null.");
                return false;
            }

            var auditModel = BuildAuditModel(correlationId, userName, DateTime.UtcNow, referenceId, type, newStateJson, oldStateJson);

            if (auditModel == null)
                return false;

            await _auditRepository.InsertAsync(auditModel);

            return true;
        }

        private static IAuditModel BuildAuditModel(
            string correlationId,
            string userName,
            DateTime timestamp,
            string referenceId,
            AuditDataType dataType,
            string newStateJson,
            string oldStateJson)
        {
            var eventType = AuditEventType.Edition;

            if (string.IsNullOrEmpty(oldStateJson))
            {
                eventType = AuditEventType.Creation;
                oldStateJson = "{}";
            }

            if (string.IsNullOrEmpty(newStateJson))
            {
                eventType = AuditEventType.Deletion;
                newStateJson = "{}";
            }

            var jdp = new JsonDiffPatch();
            var diffResult = jdp.Diff(oldStateJson, newStateJson);

            if (string.IsNullOrEmpty(diffResult))
                return null;

            return new AuditModel
            {
                Timestamp = timestamp,
                CorrelationId = correlationId,
                UserName = userName,
                Type = eventType,
                DataType = dataType,
                DataReference = referenceId,
                DataDiff = diffResult
            };
        }
    }
}