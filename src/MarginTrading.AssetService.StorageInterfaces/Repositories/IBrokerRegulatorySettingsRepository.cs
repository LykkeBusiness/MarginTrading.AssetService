using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IBrokerRegulatorySettingsRepository
    {
        Task UpdateAsync(BrokerRegulatorySettings model);
        Task<BrokerRegulatorySettings> GetByIdsAsync(Guid profileId, Guid typeId);
        Task<IReadOnlyList<BrokerRegulatorySettings>> GetAllAsync();
    }
}