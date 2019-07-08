// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
    /// <summary>
    /// MT Core service maintenance management
    /// </summary>
    [PublicAPI]
    public interface IServiceMaintenanceApi
    {
        /// <summary>
        /// Get current service state
        /// </summary>
        /// <returns></returns>
        [Get("/api/service/maintenance")]
        Task<bool> Get();

        /// <summary>
        /// Switch maintenance mode
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Post("/api/service/maintenance")]
        Task<bool> Post([Body] bool enabled);
    }
}
