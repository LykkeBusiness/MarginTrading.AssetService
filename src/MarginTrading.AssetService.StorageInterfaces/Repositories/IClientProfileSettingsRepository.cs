using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IClientProfileSettingsRepository
    {
        Task UpdateAsync(ClientProfileSettings model);
        Task<ClientProfileSettings> GetByIdsAsync(string profileId, string typeId);
        Task<List<ClientProfileSettings>> GetAllAsync(string clientProfileId, string assetTypeId);
        Task InsertMultipleAsync(IEnumerable<ClientProfileSettings> settings, TransactionContext txContext = null);
        Task<bool> WillViolateRegulationConstraintAfterRegulatorySettingsUpdateAsync(RegulatorySettingsDto regulatorySettings);
    }
}