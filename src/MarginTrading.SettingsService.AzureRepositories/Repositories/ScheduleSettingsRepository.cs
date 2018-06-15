﻿using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class ScheduleSettingsRepository : GenericAzureCrudRepository<IScheduleSettings, ScheduleSettingsEntity>,
        IScheduleSettingsRepository
    {
        public ScheduleSettingsRepository(ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager)
            : base(log, convertService, connectionStringManager, "ScheduleSettings")
        {

        }

        public new async Task<IScheduleSettings> GetAsync(string scheduleSettingsId)
        {
            return await base.GetAsync(scheduleSettingsId, ScheduleSettingsEntity.Pk);
        }

        public async Task UpdateAsync(IScheduleSettings scheduleSettings)
        {
            await base.ReplaceAsync(scheduleSettings);
        }

        public async Task DeleteAsync(string scheduleSettingsId)
        {
            await base.DeleteAsync(scheduleSettingsId);
        }
    }
}