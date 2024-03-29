﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using AuditEventType = Lykke.Snow.Audit.AuditEventType;

namespace MarginTrading.AssetService.Services
{
    public class ClientProfileSettingsService : IClientProfileSettingsService
    {
        private readonly IClientProfileSettingsRepository _regulatorySettingsRepository;
        private readonly IAuditService _auditService;
        private readonly IRegulatorySettingsApi _regulatorySettingsApi;
        private readonly ICqrsEntityChangedSender _entityChangedSender;

        public ClientProfileSettingsService(
            IClientProfileSettingsRepository regulatorySettingsRepository,
            IAuditService auditService,
            IRegulatorySettingsApi regulatorySettingsApi,
            ICqrsEntityChangedSender entityChangedSender)
        {
            _regulatorySettingsRepository = regulatorySettingsRepository;
            _auditService = auditService;
            _regulatorySettingsApi = regulatorySettingsApi;
            _entityChangedSender = entityChangedSender;
        }

        public async Task UpdateAsync(ClientProfileSettings model, string username)
        {
            var existing = await _regulatorySettingsRepository.GetByIdsAsync(model.ClientProfileId, model.AssetTypeId);

            if (existing == null)
                throw new ClientSettingsDoNotExistException();

            var regulatorySettings =
                await _regulatorySettingsApi.GetRegulatorySettingsByIdsAsync(existing.RegulatoryProfileId,
                    existing.RegulatoryTypeId);

            //This should not happen when we handle deleting of RegulatorySettings in MDM
            if (regulatorySettings.ErrorCode == RegulationsErrorCodesContract.RegulatorySettingsDoNotExist)
                throw new RegulatorySettingsDoNotExistException();

            if (model.IsAvailable && !regulatorySettings.RegulatorySettings.IsAvailable)
                throw new CannotSetToAvailableException();

            if (model.Margin > 100 || model.Margin < regulatorySettings.RegulatorySettings.MarginMinPercent)
                throw new InvalidMarginValueException();

            if (model.OnBehalfFee < 0)
                throw new InvalidOnBehalfFeeException();

            if (model.ExecutionFeesRate < 0 || model.ExecutionFeesRate > 100)
                throw new InvalidExecutionFeesRateException();

            if (model.ExecutionFeesCap < model.ExecutionFeesFloor)
                throw new InvalidExecutionFeesCapException();

            if (model.ExecutionFeesFloor > model.ExecutionFeesCap || model.ExecutionFeesFloor < 0)
                throw new InvalidExecutionFeesFloorException();

            await _regulatorySettingsRepository.UpdateAsync(model);

            await _auditService.CreateAuditRecord(AuditEventType.Edition, username, model, existing);

            await _entityChangedSender.SendEntityEditedEvent<ClientProfileSettings, ClientProfileSettingsContract, ClientProfileSettingsChangedEvent>(existing, model, username);
        }

        public Task<ClientProfileSettings> GetByIdAsync(string profileId, string typeId)
            => _regulatorySettingsRepository.GetByIdsAsync(profileId, typeId);

        public Task<List<ClientProfileSettings>> GetAllAsync()
            => _regulatorySettingsRepository.GetAllAsync(null, null);

        public Task<bool> WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(
            RegulatorySettingsDto regulatorySettings)
            => _regulatorySettingsRepository.WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(
                regulatorySettings);
    }
}