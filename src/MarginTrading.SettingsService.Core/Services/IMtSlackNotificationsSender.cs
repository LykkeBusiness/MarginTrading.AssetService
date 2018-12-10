using System.Threading.Tasks;
using Lykke.SlackNotifications;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IMtSlackNotificationsSender : ISlackNotificationsSender
    {
        Task SendRawAsync(string type, string sender, string message);
    }
}