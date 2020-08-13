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
    public class BrokerRegulatoryTypesService : IBrokerRegulatoryTypesService
    {
        private readonly IBrokerRegulatoryTypesRepository _regulatoryTypesRepository;
        private readonly IAuditService _auditService;

        public BrokerRegulatoryTypesService(
            IBrokerRegulatoryTypesRepository regulatoryTypesRepository,
            IAuditService auditService)
        {
            _regulatoryTypesRepository = regulatoryTypesRepository;
            _auditService = auditService;
        }

        public async Task InsertAsync(BrokerRegulatoryTypeWithTemplate model, string username, string correlationId)
        {
            //TODO:Check if RegulatoryTypeId exists in MDM and is from a regulation assigned to current broker

            if (model.BrokerRegulatoryTypeTemplateId.HasValue)
            {
                var regulatoryProfileTemplateExists =
                    await _regulatoryTypesRepository.ExistsAsync(model.BrokerRegulatoryTypeTemplateId.Value);

                if (!regulatoryProfileTemplateExists)
                    throw new RegulatoryTypeDoesNotExistException();
            }

            model.Id = Guid.NewGuid();

            await _regulatoryTypesRepository.InsertAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.BrokerRegulatoryType,
                model.ToJson());
        }

        public async Task UpdateAsync(BrokerRegulatoryType model, string username, string correlationId)
        {
            var existing = await _regulatoryTypesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new RegulatoryTypeDoesNotExistException();

            await _regulatoryTypesRepository.UpdateAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.BrokerRegulatoryType,
                model.ToJson(), existing.ToJson());
        }

        public async Task DeleteAsync(Guid id, string username, string correlationId)
        {
            var existing = await _regulatoryTypesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new RegulatoryTypeDoesNotExistException();

            await _regulatoryTypesRepository.DeleteAsync(id);

            await _auditService.TryAudit(correlationId, username, id.ToString(), AuditDataType.BrokerRegulatoryType,
                oldStateJson: existing.ToJson());
        }

        public Task<BrokerRegulatoryType> GetByIdAsync(Guid id)
            => _regulatoryTypesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<BrokerRegulatoryType>> GetAllAsync()
            => _regulatoryTypesRepository.GetAllAsync();
    }
}