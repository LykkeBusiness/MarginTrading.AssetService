using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using AuditEventType = Lykke.Snow.Audit.AuditEventType;

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
        private readonly ICqrsEntityChangedSender _entityChangedSender;
        private readonly string _brokerId;

        public ClientProfilesService(
            IClientProfilesRepository clientProfilesRepository,
            IAssetTypesRepository assetTypesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IAuditService auditService,
            IBrokerSettingsApi brokerSettingsApi,
            IRegulatoryProfilesApi regulatoryProfilesApi,
            IRegulatorySettingsApi regulatorySettingsApi,
            ICqrsEntityChangedSender entityChangedSender,
            string brokerId)
        {
            _clientProfilesRepository = clientProfilesRepository;
            _assetTypesRepository = assetTypesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _auditService = auditService;
            _brokerSettingsApi = brokerSettingsApi;
            _regulatoryProfilesApi = regulatoryProfilesApi;
            _regulatorySettingsApi = regulatorySettingsApi;
            _entityChangedSender = entityChangedSender;
            _brokerId = brokerId;
        }

        public async Task InsertAsync(ClientProfileWithTemplate model, string username)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            await ValidateRegulatoryProfile(model.RegulatoryProfileId, regulationId);

            List<ClientProfileSettings> clientProfileSettings;

            //duplicate settings if we use template
            if (!string.IsNullOrEmpty(model.ClientProfileTemplateId))
            {
                var regulatoryProfileTemplateExists =
                    await _clientProfilesRepository.ExistsAsync(model.ClientProfileTemplateId);

                if (!regulatoryProfileTemplateExists)
                    throw new ClientProfileDoesNotExistException();

                clientProfileSettings = await
                    _clientProfileSettingsRepository.GetAllAsync(model.ClientProfileTemplateId, null);

                foreach (var clientProfileSetting in clientProfileSettings)
                {
                    clientProfileSetting.ClientProfileId = model.Id;

                    var regulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByIdsAsync(
                        model.RegulatoryProfileId, clientProfileSetting.RegulatoryTypeId);

                    ValidateRegulatoryConstraint(regulatorySettings, clientProfileSetting);
                }
            }
            else
            {
                clientProfileSettings = new List<ClientProfileSettings>();
                var allRegulatorySettings =
                    await _regulatorySettingsApi.GetRegulatorySettingsByRegulationAsync(regulationId);
                var assetTypes = await _assetTypesRepository.GetAllAsync();

                foreach (var assetType in assetTypes)
                {
                    var regulatorySettings = allRegulatorySettings.RegulatorySettings.Single(x =>
                        x.ProfileId == model.RegulatoryProfileId && x.TypeId == assetType.RegulatoryTypeId);

                    clientProfileSettings.Add(new ClientProfileSettings
                    {
                        AssetTypeId = assetType.Id,
                        ClientProfileId = model.Id,
                        Margin = regulatorySettings.MarginMinPercent,
                        IsAvailable = regulatorySettings.IsAvailable,
                    });
                }
            }
            
            var formerDefaultProfile = await _clientProfilesRepository.InsertAsync(model, clientProfileSettings);

            await _auditService.CreateAuditRecord(AuditEventType.Creation, username, model);

            await _entityChangedSender.SendEntityCreatedEvent<ClientProfile, ClientProfileContract, ClientProfileChangedEvent>(model, username);

            if (formerDefaultProfile != null)
            {
                await AuditAndNotifyDefaultProfileChangedAsync(username, formerDefaultProfile);
            }
            
            foreach (var profileSettings in clientProfileSettings)
            {
                await _entityChangedSender.SendEntityCreatedEvent<ClientProfileSettings, ClientProfileSettingsContract, ClientProfileSettingsChangedEvent>(profileSettings, username);
            }
        }

        public async Task UpdateAsync(ClientProfile model, string username)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            await ValidateRegulatoryProfile(model.RegulatoryProfileId, regulationId);

            var existing = await _clientProfilesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new ClientProfileDoesNotExistException();

            var clientProfileSettings = await
                _clientProfileSettingsRepository.GetAllAsync(model.Id, null);

            foreach (var setting in clientProfileSettings)
            {
                var regulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByIdsAsync(
                    model.RegulatoryProfileId, setting.RegulatoryTypeId);

                ValidateRegulatoryConstraint(regulatorySettings, setting);
            }

            var formerDefaultProfile = await _clientProfilesRepository.UpdateAsync(model);

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, model, existing);
            
            await _entityChangedSender.SendEntityEditedEvent<ClientProfile, ClientProfileContract, ClientProfileChangedEvent>(existing, model, username);

            if (formerDefaultProfile != null)
            {
                await AuditAndNotifyDefaultProfileChangedAsync(username, formerDefaultProfile);
            }
        }

        public async Task DeleteAsync(string id, string username)
        {
            var existing = await _clientProfilesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new ClientProfileDoesNotExistException();

            if (existing.IsDefault)
                throw new CannotDeleteException();

            var clientProfileSettings = await
                _clientProfileSettingsRepository.GetAllAsync(id, null);

            await _clientProfilesRepository.DeleteAsync(id);

            await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, existing);

            await _entityChangedSender.SendEntityDeletedEvent<ClientProfile, ClientProfileContract, ClientProfileChangedEvent>(existing, username);
            
            foreach (var profileSettings in clientProfileSettings)
            {
                await _entityChangedSender.SendEntityDeletedEvent<ClientProfileSettings, ClientProfileSettingsContract, ClientProfileSettingsChangedEvent>(profileSettings, username);
            }
        }

        public Task<ClientProfile> GetByIdAsync(string id)
            => _clientProfilesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<ClientProfile>> GetAllAsync()
            => _clientProfilesRepository.GetAllAsync();

        public Task<ClientProfile> GetDefaultAsync()
            => _clientProfilesRepository.GetDefaultAsync();

        public Task<bool> IsRegulatoryProfileAssignedToAnyClientProfileAsync(string regulatoryProfileId)
            => _clientProfilesRepository.IsRegulatoryProfileAssignedToAnyClientProfileAsync(regulatoryProfileId);

        private async Task ValidateRegulatoryProfile(string regulatoryProfileId, string regulationId)
        {
            var regulatoryProfileResponse =
                await _regulatoryProfilesApi.GetRegulatoryProfileByIdAsync(regulatoryProfileId);

            if (regulatoryProfileResponse.ErrorCode == RegulationsErrorCodesContract.RegulatoryProfileDoesNotExist ||
                !regulatoryProfileResponse.RegulatoryProfile.RegulationId.Equals(regulationId,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new RegulatoryProfileDoesNotExistException();
        }

        [AssertionMethod]
        private static void ValidateRegulatoryConstraint(GetRegulatorySettingsByIdsResponse regulatorySettings,
            ClientProfileSettings setting)
        {
            if (regulatorySettings.ErrorCode != RegulationsErrorCodesContract.None)
                throw new RegulatorySettingsDoNotExistException();

            if (!regulatorySettings.RegulatorySettings.IsAvailable && setting.IsAvailable ||
                regulatorySettings.RegulatorySettings.MarginMinPercent > setting.Margin)
                throw new RegulationConstraintViolationException();
        }
        
        private async Task AuditAndNotifyDefaultProfileChangedAsync(string username, ClientProfile formerDefaultProfile)
        {
            var oldValue = formerDefaultProfile.ChangeDefault(true);

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, formerDefaultProfile, oldValue);
                
            await _entityChangedSender.SendEntityEditedEvent<ClientProfile, ClientProfileContract, ClientProfileChangedEvent>(oldValue, formerDefaultProfile, username);
        }
    }
}