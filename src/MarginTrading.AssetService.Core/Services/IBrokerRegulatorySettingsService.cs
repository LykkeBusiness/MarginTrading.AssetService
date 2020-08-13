using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IBrokerRegulatorySettingsService
    {
        Task UpdateAsync(BrokerRegulatorySettings model, string username, string correlationId);
        Task<BrokerRegulatorySettings> GetByIdAsync(Guid profileId, Guid typeId);
        Task<IReadOnlyList<BrokerRegulatorySettings>> GetAllAsync();
    }
}