﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using AuditEventType = Lykke.Snow.Audit.AuditEventType;

namespace MarginTrading.AssetService.Services
{
    public class AssetTypesService : IAssetTypesService
    {
        private readonly IAssetTypesRepository _assetTypesRepository;
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly IClientProfileSettingsRepository _clientProfileSettingsRepository;
        private readonly IAuditService _auditService;
        private readonly IBrokerSettingsApi _brokerSettingsApi;
        private readonly IRegulatoryTypesApi _regulatoryTypesApi;
        private readonly IRegulatorySettingsApi _regulatorySettingsApi;
        private readonly ICqrsEntityChangedSender _entityChangedSender;
        private readonly IUnderlyingCategoriesCache _underlyingCategoriesCache;
        private readonly string _brokerId;

        public AssetTypesService(
            IAssetTypesRepository assetTypesRepository,
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IAuditService auditService,
            IBrokerSettingsApi brokerSettingsApi,
            IRegulatoryTypesApi regulatoryTypesApi,
            IRegulatorySettingsApi regulatorySettingsApi,
            ICqrsEntityChangedSender entityChangedSender,
            IUnderlyingCategoriesCache underlyingCategoriesCache,
            string brokerId)
        {
            _assetTypesRepository = assetTypesRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _auditService = auditService;
            _brokerSettingsApi = brokerSettingsApi;
            _regulatoryTypesApi = regulatoryTypesApi;
            _regulatorySettingsApi = regulatorySettingsApi;
            _entityChangedSender = entityChangedSender;
            _underlyingCategoriesCache = underlyingCategoriesCache;
            _brokerId = brokerId;
        }

        public async Task InsertAsync(AssetTypeWithTemplate model, string username)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            await ValidateUnderlyingCategory(model.UnderlyingCategoryId);

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            await ValidateRegulatoryType(model.RegulatoryTypeId, regulationId);

            List<ClientProfileSettings> clientProfileSettings;

            //duplicate settings if we use template
            if (!string.IsNullOrEmpty(model.AssetTypeTemplateId))
            {
                var regulatoryProfileTemplateExists =
                    await _assetTypesRepository.ExistsAsync(model.AssetTypeTemplateId);

                if (!regulatoryProfileTemplateExists)
                    throw new AssetTypeDoesNotExistException();

                clientProfileSettings = await
                    _clientProfileSettingsRepository.GetAllAsync(null, model.AssetTypeTemplateId);

                foreach (var clientProfileSetting in clientProfileSettings)
                {
                    clientProfileSetting.AssetTypeId = model.Id;

                    var regulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByIdsAsync(
                        clientProfileSetting.RegulatoryProfileId, model.RegulatoryTypeId);

                    ValidateRegulatoryConstraint(regulatorySettings, clientProfileSetting);
                }
            }
            else
            {
                clientProfileSettings = new List<ClientProfileSettings>();
                var allRegulatorySettings =
                    await _regulatorySettingsApi.GetRegulatorySettingsByRegulationAsync(regulationId);
                var clientProfiles = await _clientProfilesRepository.GetAllAsync();

                foreach (var clientProfile in clientProfiles)
                {
                    var regulatorySettings = allRegulatorySettings.RegulatorySettings.Single(x =>
                        x.TypeId == model.RegulatoryTypeId && x.ProfileId == clientProfile.RegulatoryProfileId);

                    clientProfileSettings.Add(new ClientProfileSettings
                    {
                        AssetTypeId = model.Id,
                        ClientProfileId = clientProfile.Id,
                        Margin = regulatorySettings.MarginMinPercent,
                        IsAvailable = regulatorySettings.IsAvailable
                    });
                }
            }

            await _assetTypesRepository.InsertAsync(model, clientProfileSettings);

            await _auditService.CreateAuditRecord(AuditEventType.Creation, username, model);

            await _entityChangedSender.SendEntityCreatedEvent<AssetType, AssetTypeContract, AssetTypeChangedEvent>(model, username);
            
            foreach (var profileSettings in clientProfileSettings)
            {
                await _entityChangedSender
                    .SendEntityCreatedEvent<ClientProfileSettings, ClientProfileSettingsContract, ClientProfileSettingsChangedEvent>(profileSettings, username);
            }
        }

        public async Task UpdateAsync(AssetType model, string username)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            await ValidateUnderlyingCategory(model.UnderlyingCategoryId);

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            await ValidateRegulatoryType(model.RegulatoryTypeId, regulationId);

            var existing = await _assetTypesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new AssetTypeDoesNotExistException();

            var clientProfileSettings = await
                _clientProfileSettingsRepository.GetAllAsync(null, model.Id);

            foreach (var setting in clientProfileSettings)
            {
                var regulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByIdsAsync(
                    setting.RegulatoryProfileId, model.RegulatoryTypeId);

                ValidateRegulatoryConstraint(regulatorySettings, setting);
            }

            await _assetTypesRepository.UpdateAsync(model);

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, model, existing);

            await _entityChangedSender.SendEntityEditedEvent<AssetType, AssetTypeContract, AssetTypeChangedEvent>(existing, model, username);
        }

        public async Task DeleteAsync(string id, string username)
        {
            var existing = await _assetTypesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new AssetTypeDoesNotExistException();

            if (await _assetTypesRepository.AssignedToAnyProductAsync(id))
                throw new CannotDeleteAssetTypeAssignedToAnyProductException();

            var clientProfileSettings = await
                _clientProfileSettingsRepository.GetAllAsync(null, id);

            await _assetTypesRepository.DeleteAsync(id);

            await _auditService.CreateAuditRecord(AuditEventType.Deletion, username, existing);

            await _entityChangedSender.SendEntityDeletedEvent<AssetType, AssetTypeContract, AssetTypeChangedEvent>(existing, username);
            
            foreach (var profileSettings in clientProfileSettings)
            {
                await _entityChangedSender.SendEntityDeletedEvent<ClientProfileSettings, ClientProfileSettingsContract, ClientProfileSettingsChangedEvent>(profileSettings, username);
            }
        }

        public Task<AssetType> GetByIdAsync(string id)
            => _assetTypesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<AssetType>> GetAllAsync()
            => _assetTypesRepository.GetAllAsync();

        public Task<bool> IsRegulatoryTypeAssignedToAnyAssetTypeAsync(string regulatoryTypeId)
            => _assetTypesRepository.IsRegulatoryTypeAssignedToAnyAssetTypeAsync(regulatoryTypeId);

        private async Task ValidateRegulatoryType(string regulatoryTypeId, string regulationId)
        {
            var regulatoryTypeResponse =
                await _regulatoryTypesApi.GetRegulatoryTypeByIdAsync(regulatoryTypeId);

            if (regulatoryTypeResponse.ErrorCode == RegulationsErrorCodesContract.RegulatoryTypeDoesNotExist ||
                !regulatoryTypeResponse.RegulatoryType.RegulationId.Equals(regulationId,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new RegulatoryTypeDoesNotExistException();
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

        private async Task ValidateUnderlyingCategory(string underlyingCategoryId)
        {
            var allCategories =
                await _underlyingCategoriesCache.Get();

            if(!allCategories.Any(x => x.Id == underlyingCategoryId))
                throw new UnderlyingCategoryDoesNotExistException();
        }
    }
}