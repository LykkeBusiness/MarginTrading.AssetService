using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Common.MsSql;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Exceptions;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

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
        private readonly ITransactionRunner _transactionRunner;
        private readonly string _brokerId;

        public AssetTypesService(
            IAssetTypesRepository assetTypesRepository,
            IClientProfilesRepository clientProfilesRepository,
            IClientProfileSettingsRepository clientProfileSettingsRepository,
            IAuditService auditService,
            IBrokerSettingsApi brokerSettingsApi,
            IRegulatoryTypesApi regulatoryTypesApi,
            IRegulatorySettingsApi regulatorySettingsApi,
            ITransactionRunner transactionRunner,
            string brokerId)
        {
            _assetTypesRepository = assetTypesRepository;
            _clientProfilesRepository = clientProfilesRepository;
            _clientProfileSettingsRepository = clientProfileSettingsRepository;
            _auditService = auditService;
            _brokerSettingsApi = brokerSettingsApi;
            _regulatoryTypesApi = regulatoryTypesApi;
            _regulatorySettingsApi = regulatorySettingsApi;
            _transactionRunner = transactionRunner;
            _brokerId = brokerId;
        }

        public async Task InsertAsync(AssetTypeWithTemplate model, string username, string correlationId)
        {
            var brokerSettingsResponse = await _brokerSettingsApi.GetByIdAsync(_brokerId);

            if (brokerSettingsResponse.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
                throw new BrokerSettingsDoNotExistException();

            var regulationId = brokerSettingsResponse.BrokerSettings.RegulationId;

            var regulatoryTypeResponse =
                await _regulatoryTypesApi.GetRegulatoryTypeByIdAsync(model.RegulatoryTypeId);

            if (regulatoryTypeResponse.ErrorCode == RegulationsErrorCodesContract.RegulatoryTypeDoesNotExist ||
                regulatoryTypeResponse.RegulatoryType.RegulationId != regulationId)
                throw new RegulatoryTypeDoesNotExistException();

            model.Id = Guid.NewGuid();

            List<ClientProfileSettings> clientProfileSettings;

            //duplicate settings if we use template
            if (model.AssetTypeTemplateId.HasValue)
            {
                var regulatoryProfileTemplateExists =
                    await _assetTypesRepository.ExistsAsync(model.AssetTypeTemplateId.Value);

                if (!regulatoryProfileTemplateExists)
                    throw new AssetTypeDoesNotExistException();

                clientProfileSettings = await
                    _clientProfileSettingsRepository.GetAllAsync(null, model.AssetTypeTemplateId);

                foreach (var clientProfileSetting in clientProfileSettings)
                {
                    clientProfileSetting.AssetTypeId = model.Id;
                }
            }
            else
            {
                clientProfileSettings = new List<ClientProfileSettings>();
                var allRegulatorySettings = await _regulatorySettingsApi.GetRegulatorySettingsByRegulationAsync(regulationId);
                var clientProfiles = await _clientProfilesRepository.GetAllAsync();

                foreach (var clientProfile in clientProfiles)
                {
                    var regulatorySettings = allRegulatorySettings.RegulatorySettings.Single(x =>
                        x.TypeId == model.RegulatoryTypeId && x.ProfileId == clientProfile.RegulatoryProfileId);

                    clientProfileSettings.Add(new ClientProfileSettings
                    {
                        AssetTypeId = model.Id,
                        ClientProfileId = clientProfile.Id,
                        Margin = regulatorySettings.MarginMinPercent / 100M,
                        IsAvailable = regulatorySettings.IsAvailable,
                    });
                }
            }

            await _assetTypesRepository.InsertAsync(model, clientProfileSettings);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.AssetType,
                model.ToJson());
        }

        public async Task UpdateAsync(AssetType model, string username, string correlationId)
        {
            var existing = await _assetTypesRepository.GetByIdAsync(model.Id);

            if (existing == null)
                throw new AssetTypeDoesNotExistException();
            model.RegulatoryTypeId = existing.RegulatoryTypeId;
            await _assetTypesRepository.UpdateAsync(model);

            await _auditService.TryAudit(correlationId, username, model.Id.ToString(), AuditDataType.AssetType,
                model.ToJson(), existing.ToJson());
        }

        public async Task DeleteAsync(Guid id, string username, string correlationId)
        {
            var existing = await _assetTypesRepository.GetByIdAsync(id);

            if (existing == null)
                throw new AssetTypeDoesNotExistException();

            await _assetTypesRepository.DeleteAsync(id);

            await _auditService.TryAudit(correlationId, username, id.ToString(), AuditDataType.AssetType,
                oldStateJson: existing.ToJson());
        }

        public Task<AssetType> GetByIdAsync(Guid id)
            => _assetTypesRepository.GetByIdAsync(id);

        public Task<IReadOnlyList<AssetType>> GetAllAsync()
            => _assetTypesRepository.GetAllAsync();
    }
}