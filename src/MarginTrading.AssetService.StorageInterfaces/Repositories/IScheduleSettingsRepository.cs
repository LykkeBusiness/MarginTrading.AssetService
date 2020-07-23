// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.StorageInterfaces.Repositories
{
    public interface IScheduleSettingsRepository
    {
        Task<IReadOnlyList<IScheduleSettings>> GetFilteredAsync(string marketId = null);
        Task<IScheduleSettings> GetAsync(string scheduleSettingsId);
        Task<bool> TryInsertAsync(IScheduleSettings scheduleSettings);
        Task UpdateAsync(IScheduleSettings scheduleSettings);
        Task DeleteAsync(string scheduleSettingsId);
    }
}
