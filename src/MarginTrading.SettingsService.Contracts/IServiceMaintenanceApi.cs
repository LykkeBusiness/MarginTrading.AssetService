using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
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
        Task<bool> Post(
            [Body] bool enabled);

    }
}
