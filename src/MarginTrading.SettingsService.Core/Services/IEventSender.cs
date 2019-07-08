// Copyright (c) 2019 Lykke Corp.

using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IEventSender
    {
        Task SendSettingsChangedEvent(string route, SettingsChangedSourceType sourceType, string changedEntityId = null);
    }
}