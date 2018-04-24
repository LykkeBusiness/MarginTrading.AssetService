using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Client.Scheduling;
using Refit;

namespace MarginTrading.SettingsService.Client
{
    [PublicAPI]
    public interface IScheduleSettingsApi
    {
        [Get("/api/scheduleSettings")]
        Task<List<ScheduleSettingsContract>> List();


        [Post("/api/scheduleSettings")]
        Task<ScheduleSettingsContract> Insert(
            [Body] ScheduleSettingsContract scheduleSetting);


        [Get("/api/scheduleSettings/{settingId}")]
        Task<ScheduleSettingsContract> Get(
            [NotNull] string settingId);


        [Put("/api/scheduleSettings/{settingId}")]
        Task<ScheduleSettingsContract> Update(
            [NotNull] string settingId,
            [Body] ScheduleSettingsContract scheduleSetting);


        [Delete("/api/scheduleSettings/{settingId}")]
        Task Delete(
            [NotNull] string settingId);


        [Post("/api/scheduleSettings/compiled")]
        Task<List<CompiledScheduleContract>> StateList(
            [Body] string[] assetPairIds);

    }
}
