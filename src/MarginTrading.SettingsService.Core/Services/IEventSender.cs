using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.Core.Domain;
using Microsoft.Extensions.Internal;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IEventSender
    {
        Task SendSettingsChangedEvent(string route, SettingsChangedSourceType sourceType);
    }
}