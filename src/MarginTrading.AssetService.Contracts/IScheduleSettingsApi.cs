// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Scheduling;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Schedule settings management
    /// </summary>
    [PublicAPI]
    public interface IScheduleSettingsApi
    {
        /// <summary>
        /// Get the list of schedule settings. Optional filter by market may be applied.
        /// </summary>
        [Get("/api/scheduleSettings")]
        Task<List<ScheduleSettingsContract>> List([Query][CanBeNull] string marketId = null);

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

        /// <summary>
        /// Get current trading day info for markets. Platform schedule (with PlatformScheduleMarketId) overrides all others.
        /// </summary>
        /// <param name="marketIds">Optional. List of market Id's.</param>
        /// <param name="date">Timestamp of check (allows to check as at any moment of time</param>
        [Post("/api/scheduleSettings/markets-info")]
        Task<Dictionary<string, TradingDayInfoContract>> GetMarketsInfo([Body] string[] marketIds = null, [Query] DateTime? date = null);

        /// <summary>
        /// Get current platform trading info: is trading enabled, and last trading day.
        /// If interval has only date component i.e. time is 00:00:00.000, then previous day is returned.
        /// </summary>
        /// <param name="date">Timestamp of check (allows to check as at any moment of time</param>
        [Get("/api/scheduleSettings/platform-info")]
        Task<TradingDayInfoContract> GetPlatformInfo([Query] DateTime? date = null);
    }
}
