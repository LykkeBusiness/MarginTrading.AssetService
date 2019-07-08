// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.SettingsService.Core.Domain;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IEventSender
    {
        Task SendSettingsChangedEvent(string route, SettingsChangedSourceType sourceType, string changedEntityId = null);
    }
}