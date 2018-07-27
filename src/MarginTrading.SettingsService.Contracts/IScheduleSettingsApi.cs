using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Scheduling;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
    /// <summary>
    /// Schedule settings management
    /// </summary>
    [PublicAPI]
    public interface IScheduleSettingsApi
    {
        /// <summary>
        /// Get the list of schedule settings
        /// </summary>
        [Get("/api/scheduleSettings")]
        Task<List<ScheduleSettingsContract>> List();

        /// <summary>
        /// Create new schedule setting
        /// </summary>
        [Post("/api/scheduleSettings")]
        Task<ScheduleSettingsContract> Insert([Body] ScheduleSettingsContract scheduleSetting);

        /// <summary>
        /// Get the schedule setting
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/scheduleSettings/{settingId}")]
        Task<ScheduleSettingsContract> Get([NotNull] string settingId);

        /// <summary>
        /// Update the schedule setting
        /// </summary>
        [Put("/api/scheduleSettings/{settingId}")]
        Task<ScheduleSettingsContract> Update(
            [NotNull] string settingId,
            [Body] ScheduleSettingsContract scheduleSetting);

        /// <summary>
        /// Delete the schedule setting
        /// </summary>
        [Delete("/api/scheduleSettings/{settingId}")]
        Task Delete([NotNull] string settingId);

        /// <summary>
        /// Get the list of compiled schedule settings based on array of asset pairs
        /// </summary>
        /// <param name="assetPairIds">Null by default</param>
        [Post("/api/scheduleSettings/compiled")]
        Task<List<CompiledScheduleContract>> StateList([Body][CanBeNull] string[] assetPairIds);
    }
}
