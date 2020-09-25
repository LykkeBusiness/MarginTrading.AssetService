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
        /// Get the schedule setting
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/scheduleSettings/{settingId}")]
        Task<ScheduleSettingsContract> Get([NotNull] string settingId);

        /// <summary>
        /// Get current platform trading info: is trading enabled, and last trading day.
        /// If interval has only date component i.e. time is 00:00:00.000, then previous day is returned.
        /// </summary>
        /// <param name="date">Timestamp of check (allows to check as at any moment of time</param>
        [Get("/api/scheduleSettings/platform-info")]
        Task<TradingDayInfoContract> GetPlatformInfo([Query] DateTime? date = null);
    }
}
