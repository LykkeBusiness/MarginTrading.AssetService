using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace MarginTrading.SettingsService.Client
{
    [PublicAPI]
    public interface IServiceMaintenanceApi
    {
        [Get("api/service/maintenance")]
        Task<bool> Get();


        [Post("api/service/maintenance")]
        Task<bool> Post(
            [Body] bool enabled);

    }
}
