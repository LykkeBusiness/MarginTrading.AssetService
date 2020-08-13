using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class BrokerRegulatoryProfilesService : IBrokerRegulatoryProfilesService
    {
        private readonly IBrokerRegulatoryProfilesRepository _regulatoryProfilesRepository;
        private readonly IAuditService _auditService;
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly IRegulatoryProfilesApi _regulatoryProfilesApi;
        private readonly string _brokerId;

        public BrokerRegulatoryProfilesService(
            IBrokerRegulatoryProfilesRepository regulatoryProfilesRepository,
            IAuditService auditService,
            IBrokerSettingsApi brokerSettingsApi,
            IRegulatoryProfilesApi regulatoryProfilesApi,
            string brokerId)
        {
            _regulatoryProfilesRepository = regulatoryProfilesRepository;
            _auditService = auditService;
            _brokerSettingsApi = brokerSettingsApi;
            _regulatoryProfilesApi = regulatoryProfilesApi;
            _brokerId = brokerId;
        }

        public async Task InsertAsync(BrokerRegulatoryProfileWithTemplate model, string username, string correlationId)
        {
            //TODO:Check if RegulatoryProfileId exists in MDM and is from a regulation assigned to current broker

            if (model.BrokerRegulatoryProfileTemplateId.HasValue)
            {
                var regulatoryProfileTemplateExists =
                    await _regulatoryProfilesRepository.ExistsAsync(model.BrokerRegulatoryProfileTemplateId.Value);

                if (!regulatoryProfileTemplateExists)
                    throw new RegulatoryProfileDoesNotExistException();
            }

            model.Id = Guid.NewGuid();

            await _regulatoryProfilesRepository.InsertAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.BrokerRegulatoryProfile,
                model.ToJson());
        }

        public async Task UpdateAsync(BrokerRegulatoryProfile model, string username, string correlationId)
        {
            var existing = await _regulatoryProfilesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new RegulatoryProfileDoesNotExistException();

            await _regulatoryProfilesRepository.UpdateAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.BrokerRegulatoryProfile,
                model.ToJson(), existing.ToJson());
        }

        public async Task DeleteAsync(Guid id, string username, string correlationId)
        {
            var existing = await _regulatoryProfilesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new RegulatoryProfileDoesNotExistException();

            if (existing.IsDefault)
                throw new CannotDeleteException();

            await _regulatoryProfilesRepository.DeleteAsync(id);

            await _auditService.TryAudit(correlationId, username, id.ToString(), AuditDataType.BrokerRegulatoryProfile,
                oldStateJson: existing.ToJson());
        }

        public Task<BrokerRegulatoryProfile> GetByIdAsync(Guid id)
            => _regulatoryProfilesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<BrokerRegulatoryProfile>> GetAllAsync()
            => _regulatoryProfilesRepository.GetAllAsync();
    }
}