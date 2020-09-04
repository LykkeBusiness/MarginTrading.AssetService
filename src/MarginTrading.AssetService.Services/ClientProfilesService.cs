using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.MsSql;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class ClientProfilesService : IClientProfilesService
    {
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IAssetTypesRepository _assetTypesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IAuditService _auditService;
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly IRegulatoryProfilesApi _regulatoryProfilesApi;
        private readonly IRegulatorySettingsApi _regulatorySettingsApi;
        private readonly ITransactionRunner _transactionRunner;
        private readonly string _brokerId;

        public ClientProfilesService(
            IClientProfilesRepository clientProfilesRepository,
            IAssetTypesRepository assetTypesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IAuditService auditService,
            IBrokerSettingsApi brokerSettingsApi,
            IRegulatoryProfilesApi regulatoryProfilesApi,
            IRegulatorySettingsApi regulatorySettingsApi,
            ITransactionRunner transactionRunner,
            string brokerId)
        {
            _clientProfilesRepository = clientProfilesRepository;
            _assetTypesRepository = assetTypesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _auditService = auditService;
            _brokerSettingsApi = brokerSettingsApi;
            _regulatoryProfilesApi = regulatoryProfilesApi;
            _regulatorySettingsApi = regulatorySettingsApi;
            _transactionRunner = transactionRunner;
            _brokerId = brokerId;
        }

        public async Task InsertAsync(ClientProfileWithTemplate model, string username, string correlationId)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if(brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            var regulatoryProfileResponse =
                await _regulatoryProfilesApi.GetRegulatoryProfileByIdAsync(model.RegulatoryProfileId);

            if (regulatoryProfileResponse.ErrorCode == RegulationsErrorCodesContract.RegulatoryProfileDoesNotExist ||
                regulatoryProfileResponse.RegulatoryProfile.RegulationId != regulationId)
                throw new RegulatoryProfileDoesNotExistException();

            model.Id = Guid.NewGuid();

            List<ClientProfileSettings> clientProfileSettings;

            //duplicate settings if we use template
            if (model.ClientProfileTemplateId.HasValue)
            {
                var regulatoryProfileTemplateExists =
                    await _clientProfilesRepository.ExistsAsync(model.ClientProfileTemplateId.Value);

                if (!regulatoryProfileTemplateExists)
                    throw new ClientProfileDoesNotExistException();

                clientProfileSettings = await
                    _clientProfileSettingsRepository.GetAllAsync(model.ClientProfileTemplateId, null);

                foreach (var clientProfileSetting in clientProfileSettings)
                {
                    clientProfileSetting.ClientProfileId = model.Id;
                }
            }
            else
            {
                clientProfileSettings = new List<ClientProfileSettings>();
                var allRegulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByRegulationAsync(regulationId);
                var assetTypes = await _assetTypesRepository.GetAllAsync();

                foreach (var assetType in assetTypes)
                {
                    var regulatorySettings = allRegulatorySettings.RegulatorySettings.Single(x =>
                        x.ProfileId == model.RegulatoryProfileId && x.TypeId == assetType.RegulatoryTypeId);

                    clientProfileSettings.Add(new ClientProfileSettings
                    {
                        AssetTypeId = assetType.Id,
                        ClientProfileId = model.Id,
                        Margin = regulatorySettings.MarginMinPercent / 100M,
                        IsAvailable = regulatorySettings.IsAvailable,
                    });
                }
            }

            await _clientProfilesRepository.InsertAsync(model, clientProfileSettings);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.ClientProfile,
                model.ToJson());
        }

        public async Task UpdateAsync(ClientProfile model, string username, string correlationId)
        {
            var existing = await _clientProfilesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new ClientProfileDoesNotExistException();
            model.RegulatoryProfileId = existing.RegulatoryProfileId;
            await _clientProfilesRepository.UpdateAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.ClientProfile,
                model.ToJson(), existing.ToJson());
        }

        public async Task DeleteAsync(Guid id, string username, string correlationId)
        {
            var existing = await _clientProfilesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new ClientProfileDoesNotExistException();

            if (existing.IsDefault)
                throw new CannotDeleteException();

            await _clientProfilesRepository.DeleteAsync(id);

            await _auditService.TryAudit(correlationId, username, id.ToString(), AuditDataType.ClientProfile,
                oldStateJson: existing.ToJson());
        }

        public Task<ClientProfile> GetByIdAsync(Guid id)
            => _clientProfilesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<ClientProfile>> GetAllAsync()
            => _clientProfilesRepository.GetAllAsync();
    }
}