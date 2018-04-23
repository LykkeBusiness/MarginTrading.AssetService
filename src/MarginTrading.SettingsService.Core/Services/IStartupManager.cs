using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}