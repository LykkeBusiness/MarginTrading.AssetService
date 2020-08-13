using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class BrokerRegulatorySettingsService : IBrokerRegulatorySettingsService
    {
        private readonly IBrokerRegulatorySettingsRepository _regulatorySettingsRepository;
        private readonly IAuditService _auditService;

        public BrokerRegulatorySettingsService(IBrokerRegulatorySettingsRepository regulatorySettingsRepository, IAuditService auditService)
        {
            _regulatorySettingsRepository = regulatorySettingsRepository;
            _auditService = auditService;
        }

        public async Task UpdateAsync(BrokerRegulatorySettings model, string username, string correlationId)
        {
            var existing = await _regulatorySettingsRepository.GetByIdsAsync(model.BrokerProfileId, model.BrokerTypeId);

            if (existing == null)
                throw new BrokerRegulatorySettingsDoNotExistException();

            //TODO: Add validations about MarginMin and IsAvailable from MDM and throw concrete exceptions and catch everything in the controllers
            if (model.PhoneFees <= 0)
                throw new Exception();

            if (model.ExecutionFeesRate < 0 || model.ExecutionFeesRate > 100)
                throw new Exception();

            if (model.ExecutionFeesCap < model.ExecutionFeesFloor)
                throw new Exception();

            if (model.ExecutionFeesFloor > model.ExecutionFeesCap || model.ExecutionFeesFloor < 0)
                throw new Exception();


            await _regulatorySettingsRepository.UpdateAsync(model);

            var referenceId = $"BrokerProfileId:{model.BrokerProfileId},BrokerTypeId:{model.BrokerTypeId}";

            await _auditService.TryAudit(correlationId, username, referenceId, AuditDataType.BrokerRegulatorySettings,
                model.ToJson(), existing.ToJson());
        }

        public Task<BrokerRegulatorySettings> GetByIdAsync(Guid profileId, Guid typeId)
            => _regulatorySettingsRepository.GetByIdsAsync(profileId, typeId);

        public Task<IReadOnlyList<BrokerRegulatorySettings>> GetAllAsync()
            => _regulatorySettingsRepository.GetAllAsync();
    }
}